using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A combat round consists of Individuals in order by their rolled initiative taking turns until everyones initiative
/// is at 0 (-10 from individual after a turn) This class handles interfacing with the CrewMember class to perform a turn as
/// part of the round
/// </summary>
public class CombatRound
{
    private readonly CombatInitiative combatInitiative;
    private CrewMemberController currentActor;
    private object combatTarget;
    private CombatAction combatAction;

    public List<CrewMemberController> AllCombatActors { get; private set; } = new();
    private readonly int playerCrewSize;
    private readonly int enemyCrewSize;

    private readonly int roundNumber;

    // turns consist of two actions broken into 3 substeps each
    public enum TurnState
    {
        FirstActionReadying,
        FirstActionBegin,
        FirstActionResolution,
        SecondActionReadying,
        SecondActionBegin,
        SecondActionResolution,
        EndTurn
    }

    private TurnState turnState;

    // TODO: handle no enemies or dead enemies
    /// <summary>
    /// Initializes a combat round which can be stepped to advance initiative and progress towards event success or failure
    /// by neutralizing either half of the player crew or all of the enemies
    /// </summary>
    /// <param name="crew">Player crew participant</param>
    /// <param name="enemyCrew">Enemy crew participant</param>
    /// <param name="roundNumber">The round number</param>
    /// <exception cref="ArgumentException">Throws exception if there are no InitiativeIndividuals to start the first turn</exception>
    public CombatRound(CrewController crew, CrewController enemyCrew, int roundNumber)
    {
        this.roundNumber = roundNumber;
        AllCombatActors = crew.CrewMembers.Concat(enemyCrew.CrewMembers).ToList();
        playerCrewSize = crew.CrewMembers.Count;
        enemyCrewSize = enemyCrew.CrewMembers.Count;
        combatInitiative = new CombatInitiative(crew, enemyCrew);
        combatInitiative.SortInitiative();
        if (combatInitiative.HasNextInitiative() != true) throw new ArgumentException();
        currentActor = combatInitiative.NextInitiative().Actor;
        turnState = TurnState.FirstActionReadying;
    }

    public override string ToString()
    {
        CrewMemberController targetActor = ((CrewMemberController)combatTarget);
        StringBuilder sb = new();
        (string currentType, int currentCrewSize) = currentActor.IsEnemy ? ("Enemy",enemyCrewSize) : ("Player",playerCrewSize);
        if (targetActor != null)
        {
            (string targetType, int targetCrewSize) = targetActor.IsEnemy ? ("Enemy", enemyCrewSize) : ("Player", playerCrewSize);
            sb.Append($"{roundNumber}|{combatInitiative.GetCurrentInitiative().Initiative}|{currentType}:{currentActor.alias}/{currentCrewSize} > {targetType}:{targetActor.alias}/{targetCrewSize} @{combatAction}@###");
        } else
            sb.Append($"{roundNumber}|{combatInitiative.GetCurrentInitiative().Initiative}|{currentType}:{currentActor.alias}/{currentCrewSize} > null:null/null @null@###");
        foreach (var actor in combatInitiative.GetAllInitiativeIndividuals())
        {
            if (actor.Initiative < combatInitiative.GetCurrentInitiative().Initiative)
                sb.Append($"{actor.Initiative}|{actor.Actor.alias}:{actor.Actor.CurrentDamagedState}:{actor.Actor.DamageTaken}/{actor.Actor.MaxDamage}###");
        }
        sb.Remove(sb.Length - 3, 3);
        return sb.ToString();
    }

    /// <summary>
    /// The primary means by which the round is "completed". Each actor takes their turn in sub-steps until no one is left
    /// as next in initiative
    /// </summary>
    /// <returns>true ... don't ask me why</returns>
    public bool StepRound()
    {
        // if the current actor is bleeding out they take 1 damage and pass turn
        if (currentActor.CurrentDamagedState >= DamagedState.BleedingOut)
        {
            currentActor.TakeDamage(1);
            if (currentActor.CurrentDamagedState != DamagedState.Dead)
            {
                GameLog.Instance.PostMessageToLog($"{currentActor.alias} is bleeding out. {3 - (currentActor.DamageTaken - currentActor.MaxDamage)} turns left");
            }
            // if they are dead (either before or as a result of the 1 damage) then post message saying so (note: they are removed from combat on next round)
            else
            {
                GameLog.Instance.PostMessageToLog($"{currentActor.alias} has died.");
            }
            turnState = TurnState.EndTurn;
        }
        
        // note a combat turn should be generic (not necessarily attacking)
        bool isEnemyValue;
        switch (turnState)
        {
            case TurnState.FirstActionReadying:
                // FIX: tracking down the initiative skip happened on second round and when I stepped to next breakpoint from here
                isEnemyValue = combatInitiative.GetCurrentInitiative().IsEnemy;
                (combatAction, combatTarget) = currentActor.ChooseActionAndTarget(AllCombatActors, isEnemyValue);
                turnState = TurnState.FirstActionBegin;
                Debug.Log(currentActor.ToString());
                break;
            case TurnState.FirstActionBegin:
                currentActor.BeginPerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.FirstActionResolution;
                break;
            case TurnState.FirstActionResolution:
                currentActor.ResolvePerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.SecondActionReadying;
                Debug.Log(combatTarget.ToString());
                break;
            case TurnState.SecondActionReadying:
                isEnemyValue = combatInitiative.GetCurrentInitiative().IsEnemy;
                (combatAction, combatTarget) = currentActor.ChooseActionAndTarget(AllCombatActors, isEnemyValue);
                turnState = TurnState.SecondActionBegin;
                Debug.Log(currentActor.ToString());
                break;
            case TurnState.SecondActionBegin:
                currentActor.BeginPerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.SecondActionResolution;
                break;
            case TurnState.SecondActionResolution:
                currentActor.ResolvePerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.EndTurn;
                Debug.Log(combatTarget.ToString());
                break;
            case TurnState.EndTurn:
                // add to initiative end. This also essentially removes them from the rest of the turn if they are bleeding out or dead
                if (currentActor.CurrentDamagedState < DamagedState.BleedingOut) combatInitiative.SubtractInitiativeAndAddToEnd(10);
                FinishedTurnNextInitiative();
                Debug.Log(combatInitiative.ToString());
                break;
        }
        return true;
    }


    private void FinishedTurnNextInitiative()
    {
        if (!combatInitiative.HasNextInitiative())
        {
            RoundComplete();
            return;
        }
        currentActor = combatInitiative.NextInitiative().Actor;
        turnState = TurnState.FirstActionReadying;
    }

    /// <summary>
    /// Checks if round is complete by checking if there is someone next to take a turn
    /// </summary>
    /// <returns>Is round complete</returns>
    public bool RoundComplete() { return !combatInitiative.HasNextInitiative(); }

    /// <summary>
    /// Checks if someone won by checking if either all of the enemies are
    /// dead or dying, or if over half the players are dead or dying
    /// If for some reason both crews lost at the same time, the enemies lose
    /// </summary>
    /// <returns>1 if players have won, 2 if the enemies have won and 0 if no one has won</returns>
    public int HasSomeoneWon()
    {
        int numberOfPlayersLeft = 0;
        int numberOfEnemiesLeft = 0;
        
        foreach(CrewMemberController actor in AllCombatActors)
        {
            if (actor.CurrentDamagedState >= DamagedState.BleedingOut) continue;
            if (actor.IsEnemy) numberOfEnemiesLeft++;
            else numberOfPlayersLeft++;
        }

        if (numberOfEnemiesLeft <= 0) return 1; //Mathf.FloorToInt((float)enemyCrewSize / 2)) return 1;
        if (numberOfPlayersLeft <= Mathf.FloorToInt((float)playerCrewSize / 2)) return 2;

        return 0;
    }
}
