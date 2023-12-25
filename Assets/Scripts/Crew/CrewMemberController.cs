using Assets.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMemberController : MonoBehaviour
{
    public Attributes attributes;
    [SerializeField]
    private int health;
    [Header("Equipped")]
    public Equipped.TestPistol pistol;
    public Equipped.TestArmor armor;

    private void OnValidate()
    {
        health = attributes.body * attributes.strength;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class Attributes
    {
        [Range(1, 20)]
        public int body, agility, reaction, strength;

        [Range(1, 20)]
        public int willpower, logic, intuition, charisma;

        [Range(1, 20)]
        public int luck, initiative;

        public Attributes(int body, int agility, int reaction, int strength, int willpower, int logic, int intuition, int charisma, int luck, int initiative)
        {
            this.body = body;
            this.agility = agility;
            this.reaction = reaction;
            this.strength = strength;
            this.willpower = willpower;
            this.logic = logic;
            this.intuition = intuition;
            this.charisma = charisma;
            this.luck = luck;
            this.initiative = initiative;
        }
    }

    [System.Serializable]
    public class Equipped
    {
        private Pistol ePistol;
        private Armor eArmor;

        public Equipped(TestPistol pistol, TestArmor armor)
        {
            ePistol = pistol.GetPistol();
            eArmor = armor.GetArmor();
        }

        [System.Serializable]
        public class TestPistol
        {
            public string name;
            public int accuracy;
            public int damage;
            public int armor_penetration;
            public PistolsModes modes;
            public int recoil;
            public int ammo;

            private Pistol pistol;

            public TestPistol()
            {
                pistol = new Pistol("0135061", name, accuracy, damage, armor_penetration, modes, recoil, ammo);
            }

            public Pistol GetPistol() { return pistol; }
        }

        [System.Serializable]
        public class TestArmor
        {
            public string name;
            public int defense;
            public int weight;

            private Armor armor;

            public TestArmor()
            {
                armor = new Armor("4651668", name, defense, weight);
            }

            public Armor GetArmor() { return armor; }
        }
    }
}
