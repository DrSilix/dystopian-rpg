using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct Vector2IntTools
{
    public static int manhattanTo(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }

    public static int xDistanceTo(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(b.x - a.x);
    }
}
