using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlexUtils
{
    public static int DivideAndRoundUpInts(int a, int b)
    {
        return Mathf.CeilToInt((float)a / b);
    }
}
