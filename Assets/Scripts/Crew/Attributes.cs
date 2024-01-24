using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The 8 core attributes plus the generated luck attribute
/// </summary>
public enum Attribute
{
    body,
    agility,
    reaction,
    strength,
    willpower,
    logic,
    intuition,
    charisma,
    luck
}


/// <summary>
/// Contains attribute values for crew members and handles management of them
/// </summary>
[System.Serializable]
public class Attributes : IEnumerable
{
    [Range(1, 6)]
    public int body, agility, reaction, strength;

    [Range(1, 6)]
    public int willpower, logic, intuition, charisma;

    public int luck;

    public Attributes(int body, int agility, int reaction, int strength, int willpower, int logic, int intuition, int charisma)
    {
        this.body = body;
        this.agility = agility;
        this.reaction = reaction;
        this.strength = strength;
        this.willpower = willpower;
        this.logic = logic;
        this.intuition = intuition;
        this.charisma = charisma;
        CalculateLuck();
    }

    public IEnumerator<int> GetEnumerable()
    {
        List<int> enumerator = new()
        {
            body,
            agility,
            reaction,
            strength,
            willpower,
            logic,
            intuition,
            charisma
        };

        return enumerator.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return GetEnumerable();
    }

    /// <summary>
    /// attributes in simple string form, mostly for debug.
    /// </summary>
    /// <returns>string in the format #/#/#/#/#/#/#/#</returns>
    public override string ToString()
    {
        string result = new(
            $"{body}/" +
            $"{agility}/" +
            $"{reaction}/" +
            $"{strength}/" +
            $"{willpower}/" +
            $"{logic}/" +
            $"{intuition}/" +
            $"{charisma}");
        return result;
    }

    /// <summary>
    /// Luck is a factor of the speed of movement(agility), reaction to a situation, and the ability to know or sense an outcome beforehand(intuition)
    /// </summary>
    private void CalculateLuck() { luck = Mathf.CeilToInt(((float)(agility + reaction + intuition)) / 3); }

    /// <summary>
    /// Gets the value of the attribute requested
    /// </summary>
    /// <param name="attribute">Attribute enum representing attribute value to return</param>
    /// <returns>the attribute value or 0 if for some reason an invalid parameter gets through</returns>
    public int Get(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.body: return body;
            case Attribute.agility: return agility;
            case Attribute.reaction: return reaction;
            case Attribute.strength: return strength;
            case Attribute.willpower: return willpower;
            case Attribute.logic: return logic;
            case Attribute.intuition: return intuition;
            case Attribute.charisma: return charisma;
            case Attribute.luck: return luck;
            default: return 0;
        }
    }

    /// <summary>
    /// Gets the value of the attribute requested
    /// </summary>
    /// <param name="attribute">String representing attribute to return</param>
    /// <returns>the attribute value or 0 if for some reason an invalid parameter gets through</returns>
    public int Get(string attribute)
    {
        switch (attribute)
        {
            case "body": return body;
            case "agility": return agility;
            case "reaction": return reaction;
            case "strength": return strength;
            case "willpower": return willpower;
            case "logic": return logic;
            case "intuition": return intuition;
            case "charisma": return charisma;
            case "luck": return luck;
            default: return 0;
        }
    }

    /// <summary>
    /// Sets attribute to value. Also handles recalculating luck if needed
    /// </summary>
    /// <param name="attribute">attribute to set</param>
    /// <param name="value">value to set attribute to</param>
    /// <returns>the value the attribute was set to</returns>
    public int Set(Attribute attribute, int value)
    {
        switch (attribute)
        {
            // TODO: need to handle recalculating health on change of body
            case Attribute.body:
                body = value;
                break;
            case Attribute.agility:
                agility = value;
                break;
            case Attribute.reaction:
                reaction = value;
                break;
            case Attribute.strength:
                strength = value;
                break;
            case Attribute.willpower:
                willpower = value;
                break;
            case Attribute.logic:
                logic = value;
                CalculateLuck();
                break;
            case Attribute.intuition:
                intuition = value;
                CalculateLuck();
                break;
            case Attribute.charisma:
                charisma = value;
                CalculateLuck();
                break;
        }
        return value;
    }
}