using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Ammunition : IInventoryItem
{
    public AmmunitionSO AmmunitionBase { get; private set; }

    public InventoryItemType InventoryItemType { get; } = InventoryItemType.Ammunition;
    public WeaponType WeaponType { get; private set; }
    public bool CanStack { get; } = true;
    public string DisplayName { get; private set; }
    public Sprite Sprite { get; private set; }
    public string Manufacturer { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public int Cost { get; private set; }
    public int Availability { get; private set; }
    public bool Illegal { get; private set; }

    public Ammunition(AmmunitionSO ammunitionSO)
    {
        AmmunitionBase = ammunitionSO;

        WeaponType = ammunitionSO.weaponType;
        DisplayName = ammunitionSO.displayName;
        Sprite = ammunitionSO.sprite;
        Manufacturer = ammunitionSO.manufacturer;
        ShortDescription = ammunitionSO.shortDescription;
        LongDescription = ammunitionSO.longDescription;
        Cost = ammunitionSO.baseCost; //TODO: update
        Availability = ammunitionSO.availability;
        Illegal = ammunitionSO.illegal;
    }

    public string ToInventoryString()
    {
        StringBuilder sb = new();
        sb.Append($"<b>Weapon Type:</b> {WeaponType}\t<b>Dmg Modifier:</b>{AmmunitionBase.damageModifier}");
        return sb.ToString();
    }
}
