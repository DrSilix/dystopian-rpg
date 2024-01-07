using Assets.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMemberController : MonoBehaviour
{
    public string alias;
    [SerializeField]
    public Attributes attributes;
    [Header("DefaultEquipment")]
    public WeaponSO defaultWeapon;
    public ArmorSO defaultArmor;

    public Equipped EquippedItems { get; private set; }

    void Start()
    {
        EquippedItems = new Equipped(new Weapon(defaultWeapon), new Armor(defaultArmor));
    }

    // TODO: this is a debugging hack, possible params will be attributes, skills, and equipment objects
    private void BuildCrewMember(int bod, int agi, int rea, int str, int wil, int log, int itn, int cha)
    {
        attributes = new Attributes(bod, agi, rea, str, wil, log, itn, cha);
    }

    public int GetAttribute(Attribute attribute) { return attributes.Get(attribute); }

    // public int GetSkillRoll(int skill) { }
    public int GetAttributeRoll (Attribute attribute, int modifier = 0)
    {
        return Roll.Basic(attributes.Get(attribute) + modifier);
    }
    public int GetAttributeRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Basic(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }
    public (int successes, int crit) GetAttributeAdvancedRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Adv(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }

    [System.Serializable]
    public class Equipped
    {
        public Weapon EquippedWeapon { get; private set; }
        public Armor EquippedArmor { get; private set; }

        public Equipped(Weapon weapon, Armor armor)
        {
            EquippedWeapon = weapon;
            EquippedArmor = armor;
        }

        public object Equip(object item)
        {
            object oldEquipped;
            switch (item)
            {
                case Weapon weaponValue:
                    oldEquipped = EquippedWeapon;
                    EquippedWeapon = weaponValue;
                    return oldEquipped;
                case Armor armorValue:
                    oldEquipped = EquippedArmor;
                    EquippedArmor = armorValue;
                    return oldEquipped;
            }
            return null;
        }
    }
}
