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
    public Vector3 PlayerCrewLocation { get; set; }
    public LogEntryType LogEntryType { get; set; }
    public Color EntryColor { get; set; } = Color.gray;
    public string ShortDescription { get; set; }
    public string Body { get; set; }

    public void MarkCrewLocation()
    {
        PlayerCrewLocation = Storyteller.Instance.Crew.transform.position;
    }
}
