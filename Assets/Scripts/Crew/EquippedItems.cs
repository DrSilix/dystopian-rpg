using System.Collections.Generic;


[System.Serializable]
public class EquippedItems
{
    public enum ItemSlot { MainWeapon, OffhandWeapon, MainArmor, SubArmor, ItemOne, ItemTwo, Ammunition}
    private Dictionary<ItemSlot, InventoryItem> equipment = new();
    private CrewMemberController linkedCrewMemberController;
        
    public Weapon EquippedWeapon
    {
        get
        {
            if (EquippedWeapon == null || EquippedWeapon.Id != equipment[ItemSlot.MainWeapon].Id)
            {
                EquippedWeapon = (Weapon)equipment[ItemSlot.MainWeapon].Item;
            }
            return EquippedWeapon;
        }
        private set { EquippedWeapon = value; }
    }
    public Armor EquippedArmor
    {
        get
        {
            if (EquippedArmor == null || EquippedArmor.Id != equipment[ItemSlot.MainArmor].Id)
            {
                EquippedArmor = (Armor)equipment[ItemSlot.MainArmor].Item;
            }
            return EquippedArmor;
        }
        private set { EquippedArmor = value; }
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

    public InventoryItem Unequip(ItemSlot slot)
    {
        InventoryItem result = equipment[slot];
        equipment[slot] = null;
        result.CurrentlyEquippedBy = null;
        return result;
    }
}
