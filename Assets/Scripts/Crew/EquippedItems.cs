using System;
using System.Collections.Generic;


[System.Serializable]
public class EquippedItems
{
    public enum ItemSlot { MainWeapon, OffhandWeapon, MainArmor, SubArmor, ItemOne, ItemTwo, Ammunition}
    private Dictionary<ItemSlot, InventoryItem> equipment = new();
    private CrewMemberController linkedCrewMemberController;

    private Weapon equippedWeapon;
    public Weapon EquippedWeapon
    {
        get
        {
            if (equipment[ItemSlot.MainWeapon] != null)
            {
                if (equippedWeapon == null || equippedWeapon.Id != equipment[ItemSlot.MainWeapon].Id)
                    equippedWeapon = (Weapon)equipment[ItemSlot.MainWeapon].Item;
            }
            else equippedWeapon = null;

            return equippedWeapon;
        }
        private set { equippedWeapon = value; }
    }
    private Armor equippedArmor;
    public Armor EquippedArmor
    {
        get
        {
            if (equipment[ItemSlot.MainArmor] != null)
            {
                if (equippedArmor == null || equippedArmor.Id != equipment[ItemSlot.MainArmor].Id)
                    equippedArmor = (Armor)equipment[ItemSlot.MainArmor].Item;
            }
            else equippedArmor = null;

            return equippedArmor;
        }
        private set { equippedArmor = value; }
    }

    /*public Equipped(Weapon weapon, Armor armor)
    {
        EquippedWeapon = weapon;
        EquippedArmor = armor;
    }*/

    public EquippedItems(InventoryItem weapon, InventoryItem armor, CrewMemberController crewMemberController)
    {
        linkedCrewMemberController = crewMemberController;
        Equip(ItemSlot.MainWeapon, weapon);
        Equip(ItemSlot.MainArmor, armor);
    }
    public EquippedItems(CrewMemberController crewMemberController)
    {
        linkedCrewMemberController = crewMemberController;
    }

    public void Equip(ItemSlot slot, InventoryItem item) {
        equipment[slot] = item;
        item.CurrentlyEquippedBy = linkedCrewMemberController;
    }

    //TODO: this needs to go away or a solution to multiple type slots need to be found
    public void Equip(InventoryItem item)
    {
        ItemSlot slot;
        switch(item.Item)
        {
            case Weapon e:
                slot = ItemSlot.MainWeapon;
                break;
            case Armor e:
                slot = ItemSlot.MainArmor;
                break;
            default:
                return;
        }
        Equip(slot, item);
    }

    public InventoryItem Unequip(ItemSlot slot)
    {
        InventoryItem result = equipment[slot];
        equipment[slot] = null;
        result.CurrentlyEquippedBy = null;
        return result;
    }

    public InventoryItem Unequip(InventoryItem item)
    {
        ItemSlot slot;
        foreach (ItemSlot s in equipment.Keys)
        {
            if (equipment[s]?.Id == item.Id)
            {
                slot = s;
                equipment[slot] = null;
                item.CurrentlyEquippedBy = null;
                return item;
            }
        }
        return null;
    }
}
