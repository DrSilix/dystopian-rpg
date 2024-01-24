using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A wrapper class for use with scriptable object contact shops allowing
/// serialized definition of an item and it's quantity
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class ShopItemSOWithQuantity<T>
{
    public T item;
    public int quantity;
}

/// <summary>
/// A game shop, it contains an inventory of items and allows purchase and sale of items
/// </summary>
public class Shop
{
    public Inventory Inventory { get; private set; }
    public float buyMultiplier = 1f;

    /// <summary>
    /// Initializes the shop with the provided inventory
    /// </summary>
    /// <param name="inventory">An inventory of items</param>
    public Shop (Inventory inventory) { Inventory = inventory; }
    /// <inheritdoc cref="Shop(Inventory)" select="summary"/>
    /// <param name="items">A list of items which implement the IInventoryInterface</param>
    public Shop(List<IInventoryItem> items) { Inventory = new Inventory(items); }
    /// <inheritdoc cref="Shop(Inventory)" select="summary"/>
    /// <param name="items">A list of InventoryItems</param>
    public Shop(List<InventoryItem> items) { Inventory = new Inventory(items); }
    /// <inheritdoc cref="Shop(Inventory)" select="summary"/>
    /// <param name="shop">A shop scriptable object which contains a list of items/quantities</param>
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
        foreach (ShopItemSOWithQuantity<AmmunitionSO> item in shop.ammuntion)
        {
            Inventory.Add(new Ammunition(item.item), item.quantity);
        }
    }

    /// <summary>
    /// Purchases an item from the shop. Removes the item from the shop inventory
    /// and adds it to the player crew. Removes the cash for the item (no checks)
    /// </summary>
    /// <param name="item">InventoryItem to purchase</param>
    /// <param name="price">Final price of the inventory item</param>
    public void PurchaseItem(InventoryItem item, int price)
    {
        Inventory.Remove(item);
        Inventory crewInventory = Storyteller.Instance.Crew.CrewInventory;
        crewInventory.Add(item);
        crewInventory.Cash -= price;
    }
}
