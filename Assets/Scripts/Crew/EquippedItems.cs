using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains the equipment associated with a crew member along with methods/data related to that
/// </summary>
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

    /// <summary>
    /// Gets the dictionary storing the equipment by a key of equip slot
    /// </summary>
    /// <returns>dictionary containing equipment (ItemSlot, InventoryItem)</returns>
    public Dictionary<ItemSlot, InventoryItem> GetEquipment()
    {
        var result = equipment.ToDictionary(entry => entry.Key, entry => entry.Value);
        return result;
    }

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

    /// <summary>
    /// Handles equipping an item to a slot
    /// </summary>
    /// <param name="slot">equip slot to put item in</param>
    /// <param name="item">item to put in slot</param>
    public void Equip(ItemSlot slot, InventoryItem item) {
        if (equipment.ContainsKey(slot) && equipment[slot] != null) Unequip(slot);
        equipment[slot] = item;
        item.CurrentlyEquippedBy = linkedCrewMemberController;
    }

    /// <summary>
    /// Removes Item from equip slot and if applicable assigns a default base weapon or armor.
    /// Weapons and armor are required to be available
    /// </summary>
    /// <param name="slot">Slot to unequip the item that's in it</param>
    /// <returns>InventoryItem that was in the slot</returns>
    public InventoryItem Unequip(ItemSlot slot)
    {
        InventoryItem result = equipment[slot];
        equipment[slot] = null;
        result.CurrentlyEquippedBy = null;
        return result;
    }

    /// <summary>
    /// Removes Item from equip slot and if applicable assigns a default base weapon or armor.
    /// Weapons and armor are required to be available
    /// </summary>
    /// <param name="item">equipped item to unequip</param>
    /// <returns>InventoryItem that was unequipped, null if item not found</returns>
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
