using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItemWithQuantity<T>
{
    public T item;
    public int quantity;
}

public class Shop
{
    public Inventory Inventory { get; private set; }
    public float buyMultiplier = 1f;

    public Shop (Inventory inventory) { Inventory = inventory; }
    public Shop(List<IInventoryItem> items) { Inventory = new Inventory(items); }
    public Shop(List<InventoryItem> items) { Inventory = new Inventory(items); }
    public Shop(ShopInventorySO shop)
    {
        Inventory = new Inventory();
        foreach (WeaponSO item in shop.weapons)
        {
            Inventory.Add(new Weapon(item));
        }
        foreach (ArmorSO item in shop.armors)
        {
            Inventory.Add(new Armor(item));
        }
        foreach (ShopItemWithQuantity<AmmunitionSO> item in shop.ammuntion)
        {
            Inventory.Add(new Ammunition(item.item), item.quantity);
        }
    }
}
