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
            if (equippedWeapon == null || equippedWeapon.Id != equipment[ItemSlot.MainWeapon].Id)
            {
                equippedWeapon = (Weapon)equipment[ItemSlot.MainWeapon].Item;
            }
            return equippedWeapon;
        }
        private set { equippedWeapon = value; }
    }
    private Armor equippedArmor;
    public Armor EquippedArmor
    {
        get
        {
            if (equippedArmor == null || equippedArmor.Id != equipment[ItemSlot.MainArmor].Id)
            {
                equippedArmor = (Armor)equipment[ItemSlot.MainArmor].Item;
            }
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

    public InventoryItem Unequip(ItemSlot slot)
    {
        InventoryItem result = equipment[slot];
        equipment[slot] = null;
        result.CurrentlyEquippedBy = null;
        return result;
    }
}
