using System;
using UnityEngine;

/// <summary>
/// Stores the metadata information used to render an event log "card" associated with an individual step of the heist.
/// </summary>
public class HeistLogEntry
{
    public HeistLogEntry ParentEntry { get; set; }
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

    /// <summary>
    /// Returns how many ancestors this log entry has "it's heirarchical level".
    /// 0 = no parent / top level
    /// </summary>
    /// <returns>integer representation of the heirarchy level</returns>
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

    /// <summary>
    /// QoL shorthand, populates currentlocation with the crews position
    /// </summary>
    public void MarkCrewLocation()
    {
        CurrentLocation = Storyteller.Instance.Crew.transform.position;
    }
}
