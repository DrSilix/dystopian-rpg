using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heist Event State
/// Unfinished, beginning, running, done with success, done with failure
/// </summary>
public enum HEventState
{
    IdleUnfinished,
    Begin,
    Running,
    DoneSuccess,
    DoneFailure
}
