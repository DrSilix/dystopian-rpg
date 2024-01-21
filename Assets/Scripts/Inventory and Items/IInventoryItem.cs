using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    public int Id {  get; }
    public InventoryItemType InventoryItemType { get; }
    public bool CanStack { get; }
    public string DisplayName { get; }
    public Sprite Sprite { get; }
    public string Manufacturer { get; }
    public string ShortDescription { get; }
    public string LongDescription { get; }
    public int Cost {  get; }
    public int Availability { get; }
    public bool Illegal { get; }

    public string ToInventoryString();
}
