using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crew Member damaged state health = low value to Dead = high value
/// </summary>
public enum DamagedState
{
    Healthy,
    Wounded,
    Critical,
    BleedingOut,
    Dead
}
