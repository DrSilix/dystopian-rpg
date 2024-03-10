using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ShopInventory")]
public class ShopInventorySO : ScriptableObject
{
    //public Contact Contact { get; private set; }
    public List<WeaponSO> weapons;
    public List<ArmorSO> armors;
    public List<ShopItemSOWithQuantity<AmmunitionSO>> ammuntion;
}
