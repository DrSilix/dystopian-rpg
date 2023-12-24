using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMemberController : MonoBehaviour
{
    public Attributes attributes;
    [SerializeField]
    private int health;

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
}
