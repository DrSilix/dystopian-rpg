using System;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public int Id { get; private set; }
    public IInventoryItem Item { get; set; }
    public Inventory Inventory { get; set; }

    private int quantity;

    public CrewMemberController CurrentlyEquippedBy { get; set; }

    public InventoryItem(IInventoryItem item, int quantity, Inventory inventory)
    {
        this.Item = item;
        this.quantity = quantity;
        Id = (int)(item.GetHashCode() * 31 * UnityEngine.Random.value);
        Inventory = inventory;
    }

    public bool Remove()
    {
        return Inventory.Remove(this);
    }

    public int GetQuantity() { return quantity; }
    public int AddQuantity(int qty)
    {
        quantity += qty;
        int newQuantity = quantity;
        if (newQuantity <= 0) Remove();
        return newQuantity;
    }
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
