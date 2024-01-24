using System;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// A wrapper class for items (e.g. Weapon, Armor) that implement the IInvnetoryItem interface.
/// This keeps track of an Id unique solely to this exact object along with the attached Inventory,
/// Quantity, and the crew member that is currently equipping it
/// </summary>
[Serializable]
public class InventoryItem
{
    public int Id { get; private set; }
    public IInventoryItem Item { get; set; }
    public Inventory Inventory { get; set; }

    private int quantity;

    public CrewMemberController CurrentlyEquippedBy { get; set; }

    /// <summary>
    /// Creates a specific unique item to place in inventories from an item
    /// Can accept a quantity
    /// </summary>
    /// <param name="item">An item that implements IInventoryItem interface</param>
    /// <param name="quantity">The quantity of the item</param>
    /// <param name="inventory">The inventory that this item is added to</param>
    public InventoryItem(IInventoryItem item, int quantity, Inventory inventory)
    {
        this.Item = item;
        this.quantity = quantity;
        // takes the custom hash from the IInventoryItem (aka Id) and tacks on a random number.
        Id = (int)(item.GetHashCode() * 31 * UnityEngine.Random.value);
        Inventory = inventory;
    }

    /// <summary>
    /// Removes this item from the inventory it is attached to
    /// </summary>
    /// <returns>Whether the operation was successful</returns>
    public bool Remove()
    {
        return Inventory.Remove(this);
    }

    public int GetQuantity() { return quantity; }
    /// <summary>
    /// Adds the specified quantity to the item
    /// Can be negative to subtract quantity
    /// Handles removing the item if no quantity left
    /// </summary>
    /// <param name="qty">The quantity to add or remove (add -)</param>
    /// <returns>the new quantity after adding. can be 0 or negative if the item was removed</returns>
    public int AddQuantity(int qty)
    {
        quantity += qty;
        int newQuantity = quantity;
        if (newQuantity <= 0) Remove();
        return newQuantity;
    }
    /// <summary>
    /// Sets the quantity of the item to a specific value
    /// </summary>
    /// <param name="qty">Quantity to set the item to, must be positive</param>
    /// <returns>The new quantity of the item or -1 if the supplied quantity was negative or 0</returns>
    public int SetQuantity(int qty)
    {
        if (qty <= 0) return -1;
        quantity = qty;
        return quantity;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
