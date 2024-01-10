using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        (string currentType, int currentCrewSize) = currentActor.IsEnemy ? ("Enemy",enemyCrewSize) : ("Player",playerCrewSize);
        (string targetType, int targetCrewSize) = targetActor.IsEnemy ? ("Enemy", enemyCrewSize) : ("Player", playerCrewSize);
        StringBuilder sb = new();
        sb.Append($"{roundNumber}|{combatInitiative.GetCurrentInitiative().Initiative}|{currentType}:{currentActor.alias}/{currentCrewSize} > {targetType}:{targetActor.alias}/{targetCrewSize} @{combatAction}@###");
        foreach (var actor in combatInitiative.GetAllInitiativeIndividuals())
        {
            if (actor.Initiative < combatInitiative.GetCurrentInitiative().Initiative)
                sb.Append($"{actor.Initiative}|{actor.Actor.alias}:{actor.Actor.CurrentDamagedState}:{actor.Actor.DamageTaken}/{actor.Actor.MaxDamage}###");
        }
        sb.Remove(sb.Length - 3, 3);
        return sb.ToString();
    }

    public bool StepRound()
    {
        if (currentActor.CurrentDamagedState == DamagedState.BleedingOut)
        {
            currentActor.TakeDamage(1);
            if (currentActor.CurrentDamagedState != DamagedState.Dead)
            {
                GameLog.Instance.PostMessageToLog($"{currentActor.alias} is bleeding out. {3 - (currentActor.DamageTaken - currentActor.MaxDamage)} turns left");
            }
            else
            {
                GameLog.Instance.PostMessageToLog($"{currentActor.alias} has died.");
            }
            turnState = TurnState.EndTurn;
        }
        if (currentActor.CurrentDamagedState == DamagedState.Dead) turnState = TurnState.EndTurn;
        
        bool isEnemyValue;
        switch (turnState)
        {
            case TurnState.FirstActionReadying:
                // FIX: tracking down the initiative skip happened on second round and when I stepped to next breakpoint from here
                isEnemyValue = combatInitiative.GetCurrentInitiative().IsEnemy;
                (combatAction, combatTarget) = currentActor.ChooseActionAndTarget(AllCombatActors, isEnemyValue);
                turnState = TurnState.FirstActionBegin;
                break;
            case TurnState.FirstActionBegin:
                currentActor.BeginPerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.FirstActionResolution;
                break;
            case TurnState.FirstActionResolution:
                currentActor.ResolvePerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.SecondActionReadying;
                break;
            case TurnState.SecondActionReadying:
                isEnemyValue = combatInitiative.GetCurrentInitiative().IsEnemy;
                (combatAction, combatTarget) = currentActor.ChooseActionAndTarget(AllCombatActors, isEnemyValue);
                turnState = TurnState.SecondActionBegin;
                break;
            case TurnState.SecondActionBegin:
                currentActor.BeginPerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.SecondActionResolution;
                break;
            case TurnState.SecondActionResolution:
                currentActor.ResolvePerformCombatActionOnTarget(combatAction, combatTarget);
                turnState = TurnState.EndTurn;
                break;
            case TurnState.EndTurn:
                // add to initiative end
                if (currentActor.CurrentDamagedState < DamagedState.BleedingOut) combatInitiative.SubtractInitiativeAndAddToEnd(10);
                FinishedTurnNextInitiative();
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

    public bool RoundComplete() { return !combatInitiative.HasNextInitiative(); }

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

        if (numberOfEnemiesLeft <= Mathf.FloorToInt((float)enemyCrewSize / 2)) return 1;
        if (numberOfPlayersLeft <= Mathf.FloorToInt((float)playerCrewSize / 2)) return 1;

        return 0;
    }
}
