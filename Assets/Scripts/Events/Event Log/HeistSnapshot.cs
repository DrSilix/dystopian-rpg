/************************************************************************************
 * A snapshot is used only on player interaction to get the information needed to
 * recalculate future LogEntries. When a snapshot is used, there are some base
 * assumptions:
 * 1. The level is loaded based on a seed
 *      - this means the node path exists with associated events and enemies in
 *        their base state
 * 2. The crew object already exists in it's pre-heist state
 * 
 * Currently player interaction that can trigger a snapshot can only fire off at a
 * minimum level of the base event completing/starting a full step (e.g. combat
 * round over, objective round over)
 * 
 * A snapshot should only store information that has changed from the previous
 * snapshot. The first snapshot will be a full copy.
 * 
 * THIS IS OBSOLETE - found a better way
 ***********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeistSnapshot
{
    public HeistSnapshot NextSnapshot { get; set; }
    public int LevelSeed {  get; private set; }
    public PlayerCrewSnapshot PlayerSnapshot { get; private set; }
    public List<CrewSnapshot> EnemyCrewSnapshots { get; private set; }

    public HeistSnapshot()
    {
        
    }

    public class PlayerCrewSnapshot
    {
        public Vector3 WorldPosition { get; set; }
        public bool isMovingToNextEvent { get; set; }
        public int AssociatedEventId { get; set; }
        public CrewSnapshot CrewSnapshot { get; set; }
    }
    public class CrewSnapshot
    {
        public int CrewId { get; set; }
        public List<CrewMemberSnapshot> CrewMemberSnapshots { get; set; }
    }
    public class CrewMemberSnapshot
    {
        public int CrewMemberId { get; set; }
        public int DamageTaken { get; set; }
        public int AmmoUsed { get; set; }
        public List<EquippedItems.ItemSlot> ItemSlotsUsed;
    }
    public class WorldSnapshot
    {
        public List<EventControllerSnapshot> eventSnapshots { get; set; }
    }
    public class EventControllerSnapshot
    {
        public int EventId { get; set; }
        public HEventType.HType EventType { get; set; }
        public HEventState State { get; set; }
        public BaseEventSnapshot EventSnapshot { get; set; }
    }
    // anything more specific than the base event would only allow interaction (and thus need snapshot to recalc future) on the base step
    public class BaseEventSnapshot
    {
        public int Successes { get; set; }
        public int Fails { get; set; }
    }
}
