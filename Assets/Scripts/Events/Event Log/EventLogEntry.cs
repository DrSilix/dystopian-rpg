using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLogEntry
{
    public EventLogEntry ParentEntry {  get; set; }
    public DateTime EntryTime { get; set; }
    public LogEntryType LogEntryType { get; set; }
    public Color EntryColor { get; set; }
    public string EntryName { get; set; }
    public string EntryBody { get; set; }
}
