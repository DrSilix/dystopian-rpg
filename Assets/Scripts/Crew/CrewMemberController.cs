using Assets.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMemberController : MonoBehaviour
{
    [SerializeField]
    public Attributes attributes;
    [Header("Equipped")]
    public Equipped.TestPistol pistol;
    public Equipped.TestArmor armor;
    //public int testTemplate = 1;

    // Start is called before the first frame update
    void Start()
    {
        //if (testTemplate == 1) BuildCrewMember(5, 2, 3, 6, 1, 1, 2, 2);
        //if (testTemplate == 2) BuildCrewMember(3, 4, 4, 1, 3, 6, 5, 4);
        //if (testTemplate == 3) BuildCrewMember(4, 3, 5, 3, 2, 2, 5, 6);
    }

    private void BuildCrewMember(int bod, int agi, int rea, int str, int wil, int log, int itn, int cha)
    {
        attributes = new Attributes(bod, agi, rea, str, wil, log, itn, cha);
    }

    // Update is called once per frame
    void Update()
    {
        
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
