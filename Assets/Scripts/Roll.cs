using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

/// <summary>
/// Utility class which assists in making shadowrun style rolls. a number of 6 sided dice are rolled
/// dice that turn up 5+ are counted as success, dice that come up 1 are failures.
/// </summary>
public static class Roll
{
    /// <summary>
    /// An advanced roll which allows for critical success and failure
    /// A critical success is when more than half the dice are successes (this takes precedent over crit fails)
    /// A critical failure is when more than half the dice are failures AND there are no successes
    /// NOTE: shadowrun also has a general failure where just half the dice+ are failures, not implemented here
    /// </summary>
    /// <param name="dice">number of d6 to roll</param>
    /// <returns>a tuple containing the number of successes and the crit (1 for success, -1 for failure, and 0 for null)</returns>
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

    /// <summary>
    /// A simple shadowrun style roll. nd6 are rolled and any dice 5+ are successes
    /// </summary>
    /// <param name="dice">number of d6 to roll</param>
    /// <returns>number of successes</returns>
    public static int Basic(int dice)
    {
        int successes = 0;
        for (int i = 0; i < dice; i++)
        {
            if (Random.Range(1, (6 + 1)) >= 5) successes++;
        }
        return successes;
    }

    /// <summary>
    /// A typical roll where the total of the dice faces are taken
    /// Used for initiative
    /// </summary>
    /// <param name="dice">number of d6 to roll</param>
    /// <returns>sum of the dice</returns>
    public static int Initiative(int dice)
    {
        int result = 0;
        for(int i = 0;i < dice; i++)
        {
            result += Random.Range(1, (6 + 1));
        }
        return result;
    }
}
