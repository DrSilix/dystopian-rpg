using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This handles maintaining the state of a single round of initiative. That is all combat actors (crew members player/enemy)
/// roll initiative to determine turn order and are maintained in a list here
/// </summary>
public class CombatInitiative
{
    readonly List<InitiativeIndividual> initiativeIndividuals;
    // FIX: possible bug when the first two have the same initiative. it skipped the second guy
    private int currentPointer = -1;

    /// <summary>
    /// Initializes an initiative round with the player crew and enemy crews.
    /// Implies you can have multiple enemy crews however nothing else is setup to support that.
    /// </summary>
    /// <param name="playerCrew">the player crew</param>
    /// <param name="enemyCrew">enemy crew/s (multiple possible)</param>
    public CombatInitiative(CrewController playerCrew, params CrewController[] enemyCrew)
    {
        initiativeIndividuals = new List<InitiativeIndividual>();
        AddCrewToInitiativeOrder(playerCrew, false);
        foreach (var crew in enemyCrew) AddCrewToInitiativeOrder(crew, true);
    }
    
    private void AddCrewToInitiativeOrder(CrewController crewController, bool isEnemy)
    {
        foreach (CrewMemberController actor in crewController.CrewMembers)
        {
            if (actor.CurrentDamagedState == DamagedState.Dead) continue;
            InitiativeIndividual guy = new()
            {
                Actor = actor,
                Initiative = actor.RollInitiative(),
                IsEnemy = isEnemy
            };
            initiativeIndividuals.Add(guy);
        }
    }

    public void SortInitiative()
    {
        initiativeIndividuals?.Sort((x, y) => {
            var result = x.Initiative.CompareTo(y.Initiative);
            if (result == 0) result = x.Actor.attributes.reaction.CompareTo(y.Actor.attributes.reaction);
            while (result == 0) result = Random.Range(0, 7).CompareTo(Random.Range(0, 7));
            return -result;
        });
        StringBuilder displayInitiative = new();
        displayInitiative.Append("Turn Order: ");
        foreach (var individual in initiativeIndividuals)
        {
            displayInitiative.Append($"{individual.Actor.alias}-{individual.Initiative}/");
        }
        displayInitiative.Remove(displayInitiative.Length - 1, 1);
        GameLog.Instance.PostMessageToLog(displayInitiative.ToString());
    }

    /// <summary>
    /// Get's all actors(crew members) in initiative
    /// </summary>
    /// <returns>A list of CrewMemberControllers sorted in initiative order</returns>
    public List<CrewMemberController> GetAllInitiativeActors()
    {
        return initiativeIndividuals.Select(a => a.Actor).ToList();
    }

    /// <summary>
    /// Get's all actors(crew members) in initiative
    /// </summary>
    /// <returns>A list of InitiativeIndividuals sorted in initiative order</returns>
    public List<InitiativeIndividual> GetAllInitiativeIndividuals()
    {
        return initiativeIndividuals;
    }

    public override string ToString()
    {
        StringBuilder displayInitiative = new();
        displayInitiative.Append($"{currentPointer}|");
        int i = 0;
        foreach (var individual in initiativeIndividuals)
        {
            if(i == currentPointer) displayInitiative.Append("[[");
            displayInitiative.Append($"{individual.Actor.alias}-{individual.Initiative}/");
            if (i == currentPointer) displayInitiative.Append("]]");
            i++;
        }
        return displayInitiative.ToString();
    }

    /// <summary>
    /// returns whether there is a next InitiativeIndividual
    /// </summary>
    /// <returns>Is there a next InitiativeIndividual</returns>
    public bool HasNextInitiative() { return currentPointer + 1 < initiativeIndividuals.Count; }

    /// <summary>
    /// Advances initiative and then returns the InitiativeIndividual at that spot
    /// </summary>
    /// <returns>returns the next InitiativeIndividual</returns>
    public InitiativeIndividual NextInitiative()
    {
        currentPointer++;
        if (currentPointer >= initiativeIndividuals.Count) return null;
        return initiativeIndividuals[currentPointer];
    }

    /// <summary>
    /// Returns the InitiativeIndividual without advancing initiative
    /// </summary>
    /// <returns>The current InitiativeIndividual</returns>
    public InitiativeIndividual GetCurrentInitiative()
    {
        return initiativeIndividuals[currentPointer];
    }

    /// <summary>
    /// This is used for shadowrun style initiative (subtract 10 from initiative, if above 0 then append to end, repeat)
    /// Worth noting that the InitiativeIndividual is appended to the end of initiative without removing them from their current spot
    /// </summary>
    /// <param name="amountToSubtract">The amount to subtract from the individuals initiative</param>
    public void SubtractInitiativeAndAddToEnd(int amountToSubtract)
    {
        initiativeIndividuals[currentPointer].Initiative -= amountToSubtract;
        if (initiativeIndividuals[currentPointer].Initiative <= 0) { return; }
        initiativeIndividuals.Add(initiativeIndividuals[currentPointer]);
    }

    /// <summary>
    /// Can be (but isn't) used to remove all dead from initiative
    /// </summary>
    /// <returns>A list of the CrewMemberControllers which were removed</returns>
    public List<CrewMemberController> RemoveDead()
    {
        List<CrewMemberController> deadList = new();
        foreach(var item in initiativeIndividuals)
        {
            if (item.Actor.CurrentDamagedState == DamagedState.Dead) initiativeIndividuals.Remove(item); deadList.Add(item.Actor);
        }
        return deadList;
    }
}

/// <summary>
/// A wrapper class for CrewMemberController which importantly contains the current individuals
/// initiative score along with whether they are an enemy to the player
/// </summary>
public class InitiativeIndividual
{
    public int Initiative { get; set; }
    public CrewMemberController Actor { get; set; }
    public bool IsEnemy { get; set; }
}
