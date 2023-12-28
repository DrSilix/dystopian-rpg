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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

// Aggregates are primarily used here so far...
public enum Aggregate { min, max, avg, sum }

public class CrewController : MonoBehaviour
{
    [SerializeField]
    private int crewHealth;
    [SerializeField]
    private int crewLuck;
    [SerializeField]
    private CrewMemberController crewMember1, crewMember2, crewMember3;

    private bool isMoving = false;
    private Vector3 origin, destination;
    private float moveStartTime, moveEndTime;
    private float currentMoveTime;

    void OnEnable()
    {
        crewHealth = 100;
        crewLuck = 12;
        Camera mainCam = Camera.main;
        mainCam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, mainCam.transform.position.z);
        mainCam.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;
        currentMoveTime += Time.deltaTime;
        // if (currentMoveTime > moveEndTime) currentMoveTime = moveEndTime;
        transform.position = Vector3.Lerp(origin, destination, currentMoveTime / moveEndTime);
        if (currentMoveTime >= moveEndTime) isMoving = false;
    }

    public void moveTo(Vector3 pos, float seconds)
    {
        isMoving = true;
        origin = transform.position;
        destination = pos;
        moveStartTime = Time.time;
        moveEndTime = seconds;
        currentMoveTime = 0f;
    }

    public void AddCrewMembers(CrewMemberController member1, CrewMemberController member2, CrewMemberController member3)
    {
        crewMember1 = member1;
        crewMember2 = member2;
        crewMember3 = member3;
    }

    public void AddCrewMembers(GameObject member1, GameObject member2, GameObject member3)
    {        
        crewMember1 = member1.GetComponent<CrewMemberController>();
        crewMember2 = member2.GetComponent<CrewMemberController>();
        crewMember3 = member3.GetComponent<CrewMemberController>();
    }

    public int GetCrewAttribute(Attribute attribute, Aggregate aggregate = Aggregate.max)
    {
        switch (aggregate) {
            case Aggregate.max:
                return Mathf.Max(crewMember1.GetAttribute(attribute), crewMember1.GetAttribute(attribute), crewMember1.GetAttribute(attribute));
            case Aggregate.min:
                return Mathf.Min(crewMember1.GetAttribute(attribute), crewMember1.GetAttribute(attribute), crewMember1.GetAttribute(attribute));
            case Aggregate.sum:
                return (crewMember1.GetAttribute(attribute) +
                        crewMember2.GetAttribute(attribute) +
                        crewMember3.GetAttribute(attribute));
            case Aggregate.avg:
                return Mathf.CeilToInt((float)GetCrewAttribute(attribute, Aggregate.sum) / 3);
        }
        return -1;
    }

    public int GetCrewAttribute(Attribute attribute, int crewMember)
    {
        if (crewMember == 1) return crewMember1.GetAttribute(attribute);
        if (crewMember == 2) return crewMember2.GetAttribute(attribute);
        return crewMember3.GetAttribute(attribute);
    }

    public CrewMemberController GetCrewMember(int crewMember)
    {
        if (crewMember == 1) return crewMember1;
        if (crewMember == 2) return crewMember2;
        return crewMember3;
    }

    // crew members (1, 2, 3)
    private int GetCrewMemberWithAttributeByAggregate(Attribute attribute1, Attribute attribute2, Aggregate aggregate = Aggregate.max)
    {
        if (aggregate == Aggregate.sum || aggregate == Aggregate.avg) return -1;
        
        int[] crewAttributes = new int[] {crewMember1.GetAttribute(attribute1) + crewMember1.GetAttribute(attribute2),
                                          crewMember2.GetAttribute(attribute1) + crewMember2.GetAttribute(attribute2),
                                          crewMember3.GetAttribute(attribute1) + crewMember3.GetAttribute(attribute2)};
        int max = -1, min = -1;
        for (int i = 0; i < 3; i++ )
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
            int crewMember = GetCrewMemberWithAttributeByAggregate(attribute1, attribute2, aggregate);
            CrewMemberController crewMemberController = null;
            if (crewMember == 1) crewMemberController = crewMember1;
            if (crewMember == 2) crewMemberController = crewMember2;
            if (crewMember == 3) crewMemberController = crewMember3;
            return (crewMemberController, Roll.Basic(GetCrewAttribute(attribute1, crewMember) + GetCrewAttribute(attribute2, crewMember) + modifier));
        }
    }

    // TODO: add the ability to ask all crew members to roll

    public int GetLuckRoll() { return Random.Range(0, 20) + crewLuck; }

    public int GetLuck() { return crewLuck; }

    public int TakeDamage(int damage) { return crewHealth -= damage; }

    public int GetHealth() { return crewHealth; }
}
