using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private void CalculateLuck() { luck = Mathf.CeilToInt(((float)(agility + reaction + intuition)) / 3); }

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

    public int Set(Attribute attribute, int value)
    {
        switch (attribute)
        {
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