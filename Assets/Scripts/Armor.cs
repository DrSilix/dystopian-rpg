using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Armor : IInventoryItem
{
    public ArmorSO ArmorBase { get; private set; }
    public InventoryItemType InventoryItemType { get; } = InventoryItemType.Armor;
    public string DisplayName { get; private set; }
    public Sprite Sprite { get; private set; }
    public string Manufacturer { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public Attribute RequiredAttribute { get; private set; }
    public int RequiredAttributeMinValue { get; private set; }
    public int RequiredAttributeMaxValue { get; private set; }
    public bool IsModifyingPiece { get; private set; }
    public string ModifyingPieceDescription { get; private set; }
    public int ArmorRating { get; private set; }
    public Attribute ModifiedAttribute { get; private set; }
    public int ModifiedAttributeValue { get; private set; }
    public int Stealthiness { get; private set; }
    public int Notability { get; private set; }
    public int Intimidation { get; private set; }
    public int ItemCapacity { get; private set; }
    public int Cost { get; private set; }
    public int Availability { get; private set; }
    public bool Illegal { get; private set; }


    public Armor(ArmorSO armorSO)
    {
        ArmorBase = armorSO;

        DisplayName = armorSO.displayName;
        Sprite = armorSO.sprite;
        Manufacturer = armorSO.manufacturer;
        ShortDescription = armorSO.shortDescription;
        LongDescription = armorSO.longDescription;
        RequiredAttribute = armorSO.requiredAttribute;
        RequiredAttributeMinValue = armorSO.requiredAttributeMinValue;
        RequiredAttributeMaxValue = armorSO.requiredAttributeMaxValue;
        IsModifyingPiece = armorSO.isModifyingPiece;
        ModifyingPieceDescription = armorSO.modifyingPieceDescription;
        ArmorRating = armorSO.armorRating;
        ModifiedAttribute = armorSO.modifiedAttribute;
        ModifiedAttributeValue = armorSO.modifiedAttributeValue;
        Stealthiness = armorSO.stealthiness;
        Notability = armorSO.notability;
        Intimidation = armorSO.intimidation;
        ItemCapacity = armorSO.itemCapacity;
        Cost = armorSO.baseCost;
        Availability = armorSO.availability;
        Illegal = armorSO.illegal;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{DisplayName}|{ArmorRating}");
        return sb.ToString();
    }

    public string ToShortString()
    {
        StringBuilder sb = new();
        sb.Append($"{DisplayName[..Mathf.Min(DisplayName.Length, 10)]}|{ArmorRating}");
        return sb.ToString();
    }
}
