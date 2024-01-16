using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InventoryItemType
{
    Weapon,
    Armor,
    Ammunition,
    ItemConsumable,
    ItemTool,
    ItemUseful,
    ItemHarmful,
}

public class Inventory
{
    private List<Weapon> weapons = new();
    private List<Armor> armors = new();

    public int Size { get; private set; } = 0;

    public void Add(Weapon weapon)
    {
        weapons.Add(weapon);
        Size++;
    }

    public void Add(Armor armor)
    {
        armors.Add(armor);
        Size++;
    }

    public bool Remove(Weapon weapon)
    {
        if (!weapons.Contains(weapon)) return false;
        weapons.Remove(weapon);
        Size--;
        return true;
    }
    public bool Remove(Armor armor)
    {
        if (!armors.Contains(armor)) return false;
        armors.Remove(armor);
        Size--;
        return true;
    }

    public bool Contains (Weapon weapon)
    {
        return weapons.Contains(weapon);
    }

    public bool Contains (Armor armor)
    {
        return armors.Contains(armor);
    }

    public class InventoryItem
    {

    }

}
