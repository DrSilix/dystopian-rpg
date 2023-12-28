using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public static class Roll
{
    public static (int successes, int crit) Adv(int dice)
    {
        int successes = 0, fails = 0;
        for (int i = 0; i < dice; i++)
        {
            var roll = Random.Range(1, (6 + 1));
            if (roll >= 5) successes++;
            if (roll == 1) fails++;
        }

        int crit = 0;
        if (fails > dice/2 + 1 && successes == 0) { crit = -1; }
        if (successes > dice / 2) { crit = 1; }

        return (successes, crit);
    }

    public static int Basic(int dice)
    {
        int successes = 0;
        for (int i = 0; i < dice; i++)
        {
            if (Random.Range(1, (6 + 1)) >= 5) successes++;
        }
        return successes;
    }
}
