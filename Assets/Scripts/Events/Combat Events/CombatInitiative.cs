using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class CombatInitiative
{
    readonly List<InitiativeIndividual> initiativeIndividuals;
    // FIX: possible bug when the first two have the same initiative. it skipped the second guy
    private int currentPointer = -1;

    // First crew controller is always the players
    public CombatInitiative(CrewController playerCrew, params CrewController[] enemyCrew)
    {
        initiativeIndividuals = new List<InitiativeIndividual>();
        AddCrewToInitiativeOrder(playerCrew, false);
        foreach (var crew in enemyCrew) AddCrewToInitiativeOrder(crew, true);
    }

    public void AddCrewToInitiativeOrder(CrewController crewController, bool isEnemy)
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

    public List<CrewMemberController> GetAllInitiativeActors()
    {
        return initiativeIndividuals.Select(a => a.Actor).ToList();
    }

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

    public bool HasNextInitiative() { return currentPointer + 1 < initiativeIndividuals.Count; }

    public InitiativeIndividual NextInitiative()
    {
        currentPointer++;
        if (currentPointer >= initiativeIndividuals.Count) return null;
        return initiativeIndividuals[currentPointer];
    }

    public InitiativeIndividual GetCurrentInitiative()
    {
        return initiativeIndividuals[currentPointer];
    }

    public void SubtractInitiativeAndAddToEnd(int amountToSubtract)
    {
        initiativeIndividuals[currentPointer].Initiative -= amountToSubtract;
        if (initiativeIndividuals[currentPointer].Initiative <= 0) { return; }
        initiativeIndividuals.Add(initiativeIndividuals[currentPointer]);
    }

    public void SubtractAndResetInitiative(int amountToSubtract)
    {
        foreach (var item in initiativeIndividuals)
        {
            if (item.Actor.CurrentDamagedState == DamagedState.Dead) return;
            item.Initiative -= amountToSubtract;
            if (item.Initiative <= 0) initiativeIndividuals.Remove(item);
        }
        currentPointer = -1;
    }

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

public class InitiativeIndividual
{
    public int Initiative { get; set; }
    public CrewMemberController Actor { get; set; }
    public bool IsEnemy { get; set; }
}
