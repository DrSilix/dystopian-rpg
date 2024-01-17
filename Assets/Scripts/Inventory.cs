using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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
    // TODO: how will inventories with quantities get initialized
    
    private List<InventoryItem> items;
    public Inventory(List<IInventoryItem> items)
    {
        items = new();
        Add(items);
    }
    public Inventory(List<InventoryItem> items) { this.items = items; }
    public Inventory()
    {
        items = new();
    }

    public int Size { get => items.Count; }

    public IInventoryItem Add(IInventoryItem item)
    {
        InventoryItem foundItem = items.Find(x => x.item.DisplayName == item.DisplayName);
        if (!item.CanStack || foundItem == null)
        {
            items.Add(new InventoryItem(item, 1));
            return item;
        }
        foundItem.quantity++;
        return foundItem.item;
    }

    public IInventoryItem Add(IInventoryItem item, int quantity)
    {
        if (!item.CanStack) return null;
        InventoryItem foundItem = items.Find(x => x.item.DisplayName == item.DisplayName);
        if (foundItem == null)
        {
            items.Add(new InventoryItem(item, quantity));
            return item;
        }
        foundItem.quantity += quantity;
        return foundItem.item;
    }

    public List<IInventoryItem> Add(List<IInventoryItem> items)
    {
        List<IInventoryItem> itemsAddedToReturn = new();
        foreach (IInventoryItem item in items)
        {
            itemsAddedToReturn.Add(Add(item));
        }
        return itemsAddedToReturn;
    }

    public bool Remove(IInventoryItem item)
    {
        InventoryItem foundItem = items.Find(x => x.item.DisplayName == item.DisplayName);
        if (foundItem == null) return false;
        items.Remove(foundItem);
        return true;
    }

    public bool Remove(InventoryItem item)
    {
        InventoryItem foundItem = items.Find(x => x == item);
        if (foundItem == null) return false;
        items.Remove(foundItem);
        return true;
    }

    public bool Contains (IInventoryItem item)
    {
        return Contains(item.DisplayName);
    }

    public bool Contains (string itemDisplayName)
    {
        return items.Exists(x => x.item.DisplayName == itemDisplayName);
    }

    public int GetQuantity (IInventoryItem item)
    {
        return GetQuantity(item.DisplayName);
    }
    public int GetQuantity(string itemDisplayName)
    {
        return items.Find(x => x.item.DisplayName == itemDisplayName).quantity;
    }

    public int ChangeQuantity(IInventoryItem item, int quantity, bool doSetQuantity = false)
    {
        if (doSetQuantity && quantity <= 0) return -1;
        InventoryItem foundItem = items.Find(x => x.item.DisplayName == item.DisplayName);
        if (foundItem == null)
        {
            if (quantity <= 0) return -1;
            Add(item, quantity);
            return quantity;
        }
        if (doSetQuantity)
        {
            foundItem.quantity = quantity;
            return quantity;
        }
        foundItem.quantity += quantity;
        int newQuantity = foundItem.quantity;
        if (newQuantity <= 0) Remove(foundItem);
        return newQuantity;
    }
    public int ChangeQuantity(string itemDisplayName, int quantity, bool doSetQuantity = false)
    {
        if (doSetQuantity && quantity <= 0) return -1;
        InventoryItem foundItem = items.Find(x => x.item.DisplayName == itemDisplayName);
        if (foundItem == null)
        {
            return -1;
        }
        if (doSetQuantity)
        {
            foundItem.quantity = quantity;
            return quantity;
        }
        foundItem.quantity += quantity;
        int newQuantity = foundItem.quantity;
        if (newQuantity <= 0) Remove(foundItem);
        return newQuantity;
    }

    public List<InventoryItem> GetAllItems()
    {
        return items;
    }

    public List<InventoryItem> GetItemsOfType(InventoryItemType type)
    {
        return items.Where(x => x.item.InventoryItemType == type).ToList();
    }

    public InventoryItem GetItemByDisplayName(string displayName)
    {
        return items.Find(x => x.item.DisplayName == displayName);
    }
}
