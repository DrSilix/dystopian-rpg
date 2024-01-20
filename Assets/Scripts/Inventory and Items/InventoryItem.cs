using System;

[Serializable]
public class InventoryItem
{
    public IInventoryItem item;
    public int quantity;

    public InventoryItem(IInventoryItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
