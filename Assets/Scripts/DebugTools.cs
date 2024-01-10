using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools
{
    public static bool InBounds(int index, int arrayLen)
    {
        return (index >= 0) && (index < arrayLen);
    }
}
