using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Item/Weapon")]
public class WeaponSO : ScriptableObject
{
    //UniqueID
    public int ID;
    //weapon type
    public WeaponType weaponType;
    //display name
    public string displayName;
    //icon
    public Sprite sprite;
    //manufacturer
    public string manufacturer;
    //short description
    public string shortDescription;
    //long description
    [TextArea(4, 4)]
    public string longDescription;
    //accuracy
    public int accuracy;
    //range/reach 
    public int range;
    //damage
    public int damage;
    //armor piercing
    public int armorPiercing;
    //mode of fire (potential multiple to switch between .. but come-on alex this is ridiculous)
    public FiringMode firingMode;
    //recoil
    public int recoil;
    //ammo capacity
    public int ammoCapacity;
    //ammunition object  I ACTUALLY wouldn't want this, the ammo type determines if it works with a weapon and the actual
    //loaded ammo is part of the crewmember equipped weaon
    //public AmmunitionSO ammunition;
    //reload time
    public int reloadTime;
    //basecost
    public int baseCost;
    //availability
    public int availability;
    //legality
    public int Illegality;
    //list of special effects
    //public WeaponEffect weaponEffect;
    //list of available attach points (top, under, barrel)
    public bool hasUpperAttachPoint;
    public bool hasLowerAttachPoint;
    public bool hasBarrelAttachPoint;
    //[Tooltip("Upper/Lower/Barrel")]
    //public bool[] attachPoints = new bool[3];
    //list of attached accessories
    //public WeaponAttachments[] weaponAttachments = new WeaponAttachments[3];
}
