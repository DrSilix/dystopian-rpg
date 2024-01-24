using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

/// <summary>
/// A game inventory. Stores items and currency and methods to manipulate those items and currency
/// </summary>
public class Inventory
{    
    public int Cash {  get; set; }
    private List<InventoryItem> items;
    
    public Inventory(List<IInventoryItem> items)
    {
        items = new();
        Add(items);
    }
    public Inventory(List<InventoryItem> items) { this.items = items; }
    public Inventory() { items = new(); }

    public int Size { get => items.Count; }

    /// <summary>
    /// Adds item to inventory
    /// </summary>
    /// <param name="item">An item that implements the IInventoryItem interface</param>
    /// <returns>The InventoryItem that was added</returns>
    public InventoryItem Add(IInventoryItem item)
    {
        InventoryItem result = new InventoryItem(item, 1, this);
        items.Add(result);
        return result;
    }
    /// <summary>
    /// Adds Item to the inventory
    /// </summary>
    /// <param name="item">An InventoryItem to add</param>
    /// <returns>The InventoryItem that was added</returns>
    public InventoryItem Add(InventoryItem item)
    {
        items.Add(item);
        return item;
    }
    /// <summary>
    /// Adds Item with quantity to the inventory
    /// </summary>
    /// <param name="item">An item that implements the IInventoryItem interface</param>
    /// <param name="quantity">The quantity of that item to add</param>
    /// <returns>The InventoryItem that was added</returns>
    public InventoryItem Add(IInventoryItem item, int quantity)
    {
        if (!item.CanStack) return null;
        InventoryItem result = new InventoryItem(item, quantity, this);
        items.Add(result);
        return result;
    }
    /// <summary>
    /// Adds a List of items to the inventory
    /// </summary>
    /// <param name="items">A list of items that implement the IInventoryItem interface</param>
    /// <returns>A list of InventoryItem's that have been added</returns>
    public List<InventoryItem> Add(List<IInventoryItem> items)
    {
        List<InventoryItem> itemsAddedToReturn = new();
        foreach (IInventoryItem item in items)
        {
            itemsAddedToReturn.Add(this.Add(item));
        }
        return itemsAddedToReturn;
    }
    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="item">The InventoryItem to remove</param>
    /// <returns>Whether the item was removed or not</returns>
    public bool Remove(InventoryItem item)
    {
        InventoryItem foundItem = items.Find(x => x == item);
        if (foundItem == null) return false;
        items.Remove(foundItem);
        return true;
    }

    /// <summary>
    /// Checks whether the item exists in inventory
    /// </summary>
    /// <param name="item">An item that implements the IInventoryItem interface</param>
    /// <returns>Whether the item exists in inventory</returns>
    public bool Contains(IInventoryItem item) { return Contains(item.DisplayName); }
    /// <inheritdoc cref="Contains(IInventoryItem)" select="summary|returns"/>
    /// <param name="itemDisplayName">The name of an item in string format</param>
    public bool Contains(string itemDisplayName) { return items.Exists(x => x.Item.DisplayName == itemDisplayName); }
    /// <inheritdoc cref="Contains(IInventoryItem)" select="summary|returns"/>
    /// <remarks>NOTE: this uses the InventoryItem Id to compare to</remarks>
    /// <param name="item">The direct InventoryItem to search for</param>
    public bool Contains(InventoryItem item) { return items.Exists(x => x.Id == item.Id); }

    /// <summary>
    /// Modifies the quantity of an item. Uses the InventoryItem methods
    /// </summary>
    /// <param name="item">InventoryItem to change quantity</param>
    /// <param name="quantity">Integer representing how to change the quantity. relative change can be +/-, absolute change must be +</param>
    /// <param name="doSetQuantity">Whether to set the quantity instead of change it relatively (default: false)</param>
    /// <returns>The new quantity after making the change or -1 if an invalid operation</returns>
    public int ChangeQuantity(InventoryItem item, int quantity, bool doSetQuantity = false)
    {
        if (doSetQuantity && quantity <= 0) return -1;
        if (doSetQuantity)
        {
            item.SetQuantity(quantity);
            return quantity;
        }
        return item.AddQuantity(quantity);
    }

    /// <summary>
    /// Generates a list of all items matching the provided type
    /// </summary>
    /// <param name="type">Type of inventory to get</param>
    /// <returns>A list of matching InventoryItems</returns>
    public List<InventoryItem> GetItemsOfType(InventoryItemType type) { return items.Where(x => x.Item.InventoryItemType == type).ToList(); }
    /// <summary>
    /// Gets all items in the inventory
    /// </summary>
    /// <returns>A list of InventoryItems</returns>
    public List<InventoryItem> GetAllItems() { return items; }
    /// <summary>
    /// Gets all items matching a IInventoryItem by the interfaces Id (NOT InventoryItem Id)
    /// This is shared among all InventoryItems that are the same underlying item
    /// </summary>
    /// <param name="item">The item that implements the interface IInventoryItem to compare to</param>
    /// <returns>A list of all InventoryItems that match the underlying IInventoryItem</returns>
    public List<InventoryItem> GetItems(IInventoryItem item) { return items.FindAll(x => x.Item.Id == item.Id); }
    /// <summary>
    /// Gets all items matching a item name
    /// </summary>
    /// <param name="displayName">DisplayName to match against</param>
    /// <returns>List of matching InventoryItems</returns>
    public List<InventoryItem> GetItems(string displayName) { return items.FindAll(x => x.Item.DisplayName == displayName); }
    /// <summary>
    /// Gets an item by its InventoryItem Id unique to specifically it (not to be confused with IInventoryItem Id unique to all of the same item)
    /// </summary>
    /// <param name="id">The InventoryItem Id to match against</param>
    /// <returns>The matching InventoryItem</returns>
    public InventoryItem GetItemByInventoryItemId(int id) { return items.Find(x => x.Id == id); }
}
