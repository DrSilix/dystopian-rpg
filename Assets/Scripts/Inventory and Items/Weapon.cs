using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

public class Weapon : IInventoryItem
{
    public int Id { get; private set; }
    public WeaponSO WeaponBase { get; private set; }
    public InventoryItemType InventoryItemType { get; } = InventoryItemType.Weapon;
    public WeaponType WeaponType { get; private set; }
    public bool CanStack { get; } = false;
    public string DisplayName { get; private set; }
    public Sprite Sprite { get; private set; }
    public string Manufacturer { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public int Accuracy { get; private set; }
    public int Range { get; private set; }
    public int Damage { get; private set; }
    public int ArmorPiercing { get; private set; }
    public FiringMode FiringMode { get; private set; }
    public int Recoil { get; private set; }

    public AmmunitionSO AmmunitionSO { get; private set; }
    public int AmmoCapacity { get; private set; }
    public int AmmoHeld { get; private set; }
    public int CurrentAmmoCount { get; private set; }

    public int ReloadTime { get; private set; }
    public int Cost { get; private set; }
    public int Availability { get; private set; }
    public bool Illegal { get; private set; }
    public bool HasUpperAttachPoint { get; private set; }
    public bool HasLowerAttachPoint { get; private set; }
    public bool HasBarrelAttachPoint { get; private set; }
    //private WeaponAttachments[] weaponAttachments;

    public Weapon (WeaponSO weaponSO)
    {
        WeaponBase = weaponSO;

        WeaponType = weaponSO.weaponType;
        DisplayName = weaponSO.displayName;
        Sprite = weaponSO.sprite;
        Manufacturer = weaponSO.manufacturer;
        ShortDescription = weaponSO.shortDescription;
        LongDescription = weaponSO.longDescription;
        Accuracy = weaponSO.accuracy;
        Range = weaponSO.range;
        Damage = weaponSO.damage;
        ArmorPiercing = weaponSO.armorPiercing;
        FiringMode = weaponSO.firingMode;
        Recoil = weaponSO.recoil;
        AmmoCapacity = weaponSO.ammoCapacity;
        ReloadTime = weaponSO.reloadTime;
        Cost = weaponSO.baseCost; //TODO: update
        Availability = weaponSO.availability;
        Illegal = weaponSO.illegal;
        HasUpperAttachPoint = weaponSO.hasUpperAttachPoint;
        HasLowerAttachPoint = weaponSO.hasLowerAttachPoint;
        HasBarrelAttachPoint = weaponSO.hasBarrelAttachPoint;

        LoadAmmunition(Storyteller.Instance.AmmunitionSOs["Regular Ammunition"], AmmoCapacity);

        Id = this.GetHashCode();
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{DisplayName}|A:{Accuracy}|R:{Range}|D:{Damage}|AP:{ArmorPiercing}|{FiringMode}|Re:{Recoil}|{CurrentAmmoCount}/{AmmoCapacity}");
        return sb.ToString();
    }

    public string ToShortString()
    {
        StringBuilder sb = new();
        sb.Append($"{DisplayName[..Mathf.Min(DisplayName.Length, 10)]}|A:{Accuracy}|D:{Damage}|AP:{ArmorPiercing}|{FiringMode}|{CurrentAmmoCount}/{AmmoCapacity}");
        return sb.ToString();
    }

    public string ToInventoryString()
    {
        StringBuilder sb = new();
        sb.Append($"<b>Type:</b> {WeaponType}\tAcc:{Accuracy} Dmg:{Damage} AP:{ArmorPiercing} {FiringMode} Ammo:{CurrentAmmoCount}/{AmmoCapacity}");
        return sb.ToString();
    }

    /// <summary>
    /// Loads an ammunition type and amount into the weapon
    /// Will only load ammo until weapon is full, will take remaining
    /// ammo and add to AmmoHeld which is currently unimplemented
    /// </summary>
    /// <param name="ammunitionSO">Ammunition scriptable object to load</param>
    /// <param name="amount">Amount to load</param>
    public void LoadAmmunition(AmmunitionSO ammunitionSO, int amount)
    {
        AmmunitionSO = ammunitionSO;
        amount += CurrentAmmoCount;
        if (amount <= AmmoCapacity) CurrentAmmoCount = amount;
        else
        {
            CurrentAmmoCount = AmmoCapacity;
            AmmoHeld += amount - AmmoCapacity;
        }
    }

    /// <summary>
    /// Simply removes the amount of ammo specified. "Fires the weapon"
    /// Does not check if the amount is available
    /// </summary>
    /// <param name="amount">Amount of ammo to remove</param>
    public void FireRounds(int amount) { CurrentAmmoCount -= amount; }

    /// <summary>
    /// Loads exactly to correct amount of ammo to fill the capacity
    /// </summary>
    public void Reload()
    {
        LoadAmmunition(Storyteller.Instance.AmmunitionSOs["Regular Ammunition"], AmmoCapacity - CurrentAmmoCount);
    }

    public override int GetHashCode()
    {
        int hash = base.GetHashCode();
        hash = hash * 31 + DisplayName.GetHashCode();
        hash = hash * 31 + WeaponBase.GetHashCode();
        // TODO: add potential modifications here
        return hash;
    }

}
