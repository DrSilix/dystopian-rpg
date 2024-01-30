using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeistLog
{
    public List<HeistLogEntry> Log;

    public HeistLog()
    {
        Log = new List<HeistLogEntry>();
    }

    public void Add(HeistLogEntry entry)
    {
        Log.Add(entry);
    }

    public HeistLogEntry GetCurrent() { return Log[Log.Count - 1]; }
    public HeistLogEntry GetPrevious() { return Log[Log.Count - 2]; }
    public DateTime GetNextTime()
    {
        HeistLogEntry previous = GetPrevious();
        return previous.EntryStartTime.AddSeconds(previous.Duration);
    }
}
