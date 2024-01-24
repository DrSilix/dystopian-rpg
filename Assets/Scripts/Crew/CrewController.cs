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

/// <summary>
/// Different ways to summarize crew members when rolling or retrieving attributes
/// </summary>
public enum Aggregate { min, max, avg, sum }

/// <summary>
/// Handles all crew collective related functionality
/// </summary>
public class CrewController : MonoBehaviour
{
    public List<CrewMemberController> CrewMembers { get; private set; } = new();
    public Inventory CrewInventory { get; private set; } = new();

    public CrewController()
    {
        CrewInventory.Cash = 5000;
    }

    /// <summary>
    /// Add a crew member to the crew. contains overload for using a game object as well as
    /// </summary>
    /// <param name="member">CrewMemberController representing the crew member</param>
    public void AddCrewMember(CrewMemberController member) {
        CrewMembers.Add(member);
        member.SetConnectedCrew(this);
        member.Initialize();
    }
    public void AddCrewMembers(params CrewMemberController[] members) { foreach(CrewMemberController c in members) AddCrewMember(c); }
    public void AddCrewMember(GameObject member) { AddCrewMember(member.GetComponent<CrewMemberController>()); }
    public void AddCrewMembers(params GameObject[] members)
    {
        foreach (GameObject c in members) AddCrewMember(c.GetComponent<CrewMemberController>());
    }

    /// <summary>
    /// Gets the aggregate of the attribute (Min, Max, Avg, Sum) across all crew members
    /// </summary>
    /// <param name="attribute">attribute to get aggregate of</param>
    /// <param name="aggregate">how to summarize all crew member values of the attribute (default: max)</param>
    /// <returns>the aggregate of the attribute or -1 if the aggregate is invalid</returns>
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
    /// <param name="crewMember">The integer [0 - (n-1)] representation of the crew member</param>
    /// <returns></returns>
    public int GetCrewAttribute(Attribute attribute, int crewMember)
    {
        return CrewMembers[crewMember].GetAttribute(attribute);
    }

    /// <summary>
    /// Gets the crew member controller by index
    /// </summary>
    /// <param name="crewMember">The index of the crew member [0 - (n-1)]</param>
    /// <returns></returns>
    public CrewMemberController GetCrewMember(int crewMember)
    {
        return CrewMembers[crewMember];
    }

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

    /// <summary>
    /// Makes a roll involving all crew members on two attributes if the aggregate is either sum or average. This simulates the crew acting as one
    /// Makes a roll on the highest performing crew member on both attributes if the aggregate is either min or max
    /// A roll is nd6 where n is the sum of the final attribute values plus the modifier. The number of successes are the dice that turn up with a value of >=5
    /// </summary>
    /// <param name="attribute1">Primary attribute involved in the roll</param>
    /// <param name="attribute2">Secondary attribute involved in the roll</param>
    /// <param name="modifier">added directly to the number of dice rolled</param>
    /// <param name="aggregate">How to aggregate the roll. Sum/Avg will roll on all crew members,
    /// Min/Max will roll on the highest performing crew member (default: average)</param>
    /// <returns>A tuple containing the highest performing crew member if applicable (otherwise null) and the result of the roll</returns>
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
