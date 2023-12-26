using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attributes
{
    [Range(1, 6)]
    public int body, agility, reaction, strength;

    [Range(1, 6)]
    public int willpower, logic, intuition, charisma;

    public int luck;
    public int initiative;
    public int health;

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
        luck = charisma + intuition - logic;
        initiative = reaction + intuition;
        health = Mathf.CeilToInt((float)8 + ((float)body / 2));
    }
}
