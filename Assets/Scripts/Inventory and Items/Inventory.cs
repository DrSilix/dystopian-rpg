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

public class Inventory
{    
    private List<InventoryItem> items;
    
    
    public Inventory(List<IInventoryItem> items)
    {
        items = new();
        Add(items);
    }
    public Inventory(List<InventoryItem> items) { this.items = items; }
    public Inventory() { items = new(); }

    public int Size { get => items.Count; }

    public InventoryItem Add(IInventoryItem item)
    {
        InventoryItem result = new InventoryItem(item, 1, this);
        items.Add(result);
        return result;
    }

    public InventoryItem Add(IInventoryItem item, int quantity)
    {
        if (!item.CanStack) return null;
        InventoryItem result = new InventoryItem(item, quantity, this);
        items.Add(result);
        return result;
    }

    public List<InventoryItem> Add(List<IInventoryItem> items)
    {
        List<InventoryItem> itemsAddedToReturn = new();
        foreach (IInventoryItem item in items)
        {
            itemsAddedToReturn.Add(this.Add(item));
        }
        return itemsAddedToReturn;
    }

    public bool Remove(InventoryItem item)
    {
        InventoryItem foundItem = items.Find(x => x == item);
        if (foundItem == null) return false;
        items.Remove(foundItem);
        return true;
    }

    public bool Contains(IInventoryItem item) { return Contains(item.DisplayName); }
    public bool Contains(string itemDisplayName) { return items.Exists(x => x.Item.DisplayName == itemDisplayName); }
    public bool Contains(InventoryItem item) { return items.Exists(x => x.Id == item.Id); }

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

    public List<InventoryItem> GetItemsOfType(InventoryItemType type) { return items.Where(x => x.Item.InventoryItemType == type).ToList(); }
    public List<InventoryItem> GetAllItems() { return items; }
    public List<InventoryItem> GetItems(IInventoryItem item) { return items.FindAll(x => x.Item.Id == item.Id); }
    public List<InventoryItem> GetItems(string displayName) { return items.FindAll(x => x.Item.DisplayName == displayName); }
    public InventoryItem GetItemByInventoryItemId(int id) { return items.Find(x => x.Id == id); }
}
