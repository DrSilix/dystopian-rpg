using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : ScriptableObject
{
    //UniqueID
    public int UUID;
    //weapon type
    public WeaponType weaponType;
    //display name
    public string displayName;
    //icon
    public Sprite sprite;
    //manufacturer
    public string manufacturer;
    //short description
    public string shortDescription;
    //long description
    [TextArea(4, 4)]
    public string longDescription;
    //accuracy
    public int accuracyModifier;
    //range/reach
    public int rangeModifier;
    //damage
    public int damageModifier;
    //armor piercing
    public int armorPiercingModifier;
    //basecost
    public int baseCost;
    //availability
    public int availability;
    //legality
    public int legality;
    //list of special effects
    //public WeaponEffect weaponEffect;
}
