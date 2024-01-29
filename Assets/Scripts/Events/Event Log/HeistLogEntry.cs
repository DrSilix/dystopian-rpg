using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeistLogEntry
{
    public HeistLogEntry ParentEntry {  get; set; }
    public HeistSnapshot HeistSnapshot { get; set; }
    public DateTime EntryStartTime { get; set; }
    public float Duration { get; set; }
    public Vector3 PlayerCrewLocation { get; set; }
    public LogEntryType LogEntryType { get; set; }
    public Color EntryColor { get; set; }
    public string EntryName { get; set; }
    public string EntryBody { get; set; }
}
