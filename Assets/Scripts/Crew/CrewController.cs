/*
 * Overall control of the crew, handles anything crew related that isn't individualistic.
 * contains a health/cohesion state (general group failure)
 * holds same stats as crew but maintains the best for each among the crew
 * handles the macro state of the game object within the game mechanics (where is the crew in world coords, etc..)
 * provides a generic interface for the individual crew members
 * holds variable amound of crew members
 * holds inventory type things for movement between events
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

// Aggregates are primarily used here so far...
public enum Aggregate { min, max, avg, sum }

public class CrewController : MonoBehaviour
{
    public List<CrewMemberController> CrewMembers { get; private set; } = new();

    public void AddCrewMember(CrewMemberController member) { CrewMembers.Add(member); }
    public void AddCrewMembers(params CrewMemberController[] members) { foreach(CrewMemberController c in members) AddCrewMember(c); }
    public void AddCrewMember(GameObject member) { CrewMembers.Add(member.GetComponent<CrewMemberController>()); }
    public void AddCrewMembers(params GameObject[] members)
    {
        foreach (GameObject c in members) AddCrewMember(c.GetComponent<CrewMemberController>());
    }

    public int GetCrewAttribute(Attribute attribute, Aggregate aggregate = Aggregate.max)
    {
        switch (aggregate) {
            case Aggregate.max:
                return CrewMembers.Select(m => m.GetAttribute(attribute)).ToArray().Max();
            case Aggregate.min:
                return CrewMembers.Select(m => m.GetAttribute(attribute)).ToArray().Min();
            case Aggregate.sum:
                return CrewMembers.Select(m => m.GetAttribute(attribute)).ToArray().Sum();
            case Aggregate.avg:
                return Mathf.CeilToInt((float)CrewMembers.Select(m => m.GetAttribute(attribute)).ToArray().Average());
        }
        return -1;
    }

    /// <summary>
    /// Gets the integer value of the indicated crew members attribute
    /// </summary>
    /// <param name="attribute">The attribute to get the value of</param>
    /// <param name="crewMember">The integer [0-n] representation of the crew member</param>
    /// <returns></returns>
    public int GetCrewAttribute(Attribute attribute, int crewMember)
    {
        return CrewMembers[crewMember].GetAttribute(attribute);
    }

    public CrewMemberController GetCrewMember(int crewMember)
    {
        return CrewMembers[crewMember];
    }

    // crew members (1, 2, 3)
    private int GetCrewMemberWithAttributeByAggregate(Attribute attribute1, Attribute attribute2, Aggregate aggregate = Aggregate.max)
    {
        if (aggregate == Aggregate.sum || aggregate == Aggregate.avg) return -1;

        int[] crewAttributes = CrewMembers.Select(m => m.GetAttribute(attribute1) + m.GetAttribute(attribute2)).ToArray();
        
        int max = -1, min = -1;
        for (int i = 0; i < crewAttributes.Length; i++ )
        {
            if (max == -1 || crewAttributes[i] > crewAttributes[max]) max = i;
            if (min == -1 || crewAttributes[i] < crewAttributes[min]) min = i;
        }

        if (aggregate == Aggregate.max) return max;
        return min;
    }

    public (CrewMemberController crewMember, int result) GetCrewRoll (Attribute attribute1, Attribute attribute2, int modifier = 0, Aggregate aggregate = Aggregate.avg) {
        if (aggregate == Aggregate.avg || aggregate == Aggregate.sum)
        {
            return (null, Roll.Basic(GetCrewAttribute(attribute1, aggregate) + GetCrewAttribute(attribute2, aggregate) + modifier));
        }
        else
        {
            int memberID = GetCrewMemberWithAttributeByAggregate(attribute1, attribute2, aggregate);
            return (CrewMembers[memberID], Roll.Basic(GetCrewAttribute(attribute1, memberID) + GetCrewAttribute(attribute2, memberID) + modifier));
        }
    }
}
