using System.Collections;
using System.Collections.Generic;
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
    luck,
    initiative,
    health
}



[System.Serializable]
public class Attributes
{
    [Range(1, 6)]
    public int body, agility, reaction, strength;

    [Range(1, 6)]
    public int willpower, logic, intuition, charisma;

    public int luck;
    public int initiative;
    public int maxHealth;

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
        CalculateInitiative();
        CalculateMaxHealth();
    }

    private void CalculateLuck() { luck = charisma + intuition - logic; }
    private void CalculateInitiative() { initiative = reaction + intuition; }
    private void CalculateMaxHealth() { maxHealth = Mathf.CeilToInt((float)8 + ((float)body / 2)); }

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
            case Attribute.initiative: return initiative;
            case Attribute.health: return maxHealth;
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
            case "initiative": return initiative;
            case "health": return maxHealth;
            default: return 0;
        }
    }

    public void Set(Attribute attribute, int value)
    {
        switch (attribute)
        {
            case Attribute.body:
                body = value;
                CalculateMaxHealth();
                break;
            case Attribute.agility:
                agility = value;
                break;
            case Attribute.reaction:
                reaction = value;
                CalculateInitiative();
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
                CalculateInitiative();
                break;
            case Attribute.charisma:
                charisma = value;
                CalculateLuck();
                break;
        }
    }
}