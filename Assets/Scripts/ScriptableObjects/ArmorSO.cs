using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Item/Armor")]
public class ArmorSO : ScriptableObject
{
    //UniqueID
    public int ID;
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
    //Required Stat
    public Attribute requiredAttribute;
    [Range(-1, 5)] public int requiredAttributeMinValue = -1;
    [Range(-1, 5)] public int requiredAttributeMaxValue = -1;
    //armor type????
    public bool isModifyingPiece;
    public string modifyingPieceDescription;
    //armor faction (police, business, street, ...
    //armor
    [Range(0, 18)]
    public int armorRating;
    //phys protection
    //elem protection
    //attribute modification
    public Attribute modifiedAttribute;
    [Range(-2, 2)]
    public int modifiedAttributeValue;
    //skill modification
    //value
    //stealthyness
    [Range(-2, 2)]
    public int stealthiness;
    //blend
    [Range(0, 3)]
    public int notability;
    //intimidation
    [Range(0, 3)]
    public int intimidation;
    //weapon capacity
    //ammo clip capacity
    [Range(0, 10)]
    public int itemCapacity;
    //item capacity
    //smart features
    //down detection
    //basecost
    public int baseCost;
    //availability
    [Range(0, 3)]
    public int availability;
    //legality
    public bool illegal;
}
