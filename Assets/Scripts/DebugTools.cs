using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools
{
    public static bool inBounds(int index, int arrayLen)
    {
        return (index >= 0) && (index < arrayLen);
    }
}
