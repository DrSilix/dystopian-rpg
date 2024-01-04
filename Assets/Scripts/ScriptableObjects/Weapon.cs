using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Item/Weapon")]
public class Weapon : ScriptableObject
{
    //UniqueID
    public int UUID;
    public bool generateID;
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
    //ammunition object
    public Ammunition ammunition;
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
    [Tooltip("Upper/Lower/Barrel")]
    public bool[] attachPoints = new bool[3];
    //list of attached accessories
    //public WeaponAttachments[] weaponAttachments = new WeaponAttachments[3];

    private void OnValidate()
    {
        if (generateID)
        {
            int hash = 31;
            hash += 13 * hash + weaponType.GetHashCode();
            hash += 13 * hash + displayName.GetHashCode();
            hash += 13 * hash + sprite.GetHashCode();
            hash += 13 * hash + manufacturer.GetHashCode();
            hash += 13 * hash + shortDescription.GetHashCode();
            hash += 13 * hash + longDescription.GetHashCode();
            hash += 13 * hash + accuracy.GetHashCode();
            hash += 13 * hash + range.GetHashCode();
            hash += 13 * hash + damage.GetHashCode();
            hash += 13 * hash + armorPiercing.GetHashCode();
            hash += 13 * hash + firingMode.GetHashCode();
            hash += 13 * hash + ammoCapacity.GetHashCode();
            hash += 13 * hash + reloadTime.GetHashCode();
            hash += 13 * hash + availability.GetHashCode();
            hash += 13 * hash + Illegality.GetHashCode();
            hash += 13 * hash + recoil.GetHashCode();
            hash += 13 * hash + ammoCapacity.GetHashCode();
            hash = Mathf.Abs(hash);
            UUID = hash;
            generateID = false;
        }
    }
}
