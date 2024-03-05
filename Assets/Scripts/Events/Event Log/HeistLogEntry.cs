using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeistLogEntry
{
    public HeistLogEntry ParentEntry {  get; set; }
    public int StepNumber { get; set; }
    public DateTime EntryStartTime { get; set; }
    public float Duration { get; set; }
    public Vector3 CurrentLocation { get; set; }
    public LogEntryType LogEntryType { get; set; }
    public Color EntryColor { get; set; } = Color.gray;
    public string ShortDescription { get; set; }
    // TODO: this should be a visual element instead of a string
    // could possibly have a default simple text visual element that can be overwritten
    public string Body { get; set; }

    public int GetHierarchyLevel()
    {
        int level = 0;
        HeistLogEntry pointer = this;
        while (pointer.ParentEntry != null)
        {
            level++;
            pointer = pointer.ParentEntry;
        }
        return level;
    }
    public void MarkCrewLocation()
    {
        CurrentLocation = Storyteller.Instance.Crew.transform.position;
    }
}
