using System;
using System.Collections.Generic;

/// <summary>
/// A datatype that stores a collection of log entries and provides methods for querying and manipulating them
/// </summary>
public class HeistLog
{
    public List<HeistLogEntry> Log;

    public HeistLog()
    {
        Log = new List<HeistLogEntry>();
    }

    /// <summary>
    /// Adds an entry to the end of the event log
    /// </summary>
    /// <param name="entry">The entry to be added to the end of the log</param>
    public void Add(HeistLogEntry entry)
    {
        Log.Add(entry);
    }

    /// <summary>
    /// Gets the most recent log entry. "Current" is used because an instance is added and then populated with data
    /// Be sure it exists, no error checking is done.
    /// </summary>
    /// <returns>The most recent log entry</returns>
    public HeistLogEntry GetCurrent() { return Log[Log.Count - 1]; }
    /// <summary>
    /// Gets the previous log entry. Be sure it exists, no error checking is done
    /// </summary>
    /// <returns>the previous log entry</returns>
    public HeistLogEntry GetPrevious() { return Log[Log.Count - 2]; }
    /// <summary>
    /// A shorthand method that will return the previous entries datetime plus the duration of the previous entry.
    /// Used to populate the time of the current entry.
    /// </summary>
    /// <returns>Datetime of next entry after previous entry duration</returns>
    public DateTime GetNextTime()
    {
        HeistLogEntry previous = GetPrevious();
        return previous.EntryStartTime.AddSeconds(previous.Duration);
    }
}
