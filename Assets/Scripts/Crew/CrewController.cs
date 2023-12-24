using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewController : MonoBehaviour
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int luck;

    void OnEnable()
    {
        health = 100;
        luck = 12;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetLuckRoll() { return Random.Range(0, 20) + luck; }

    public int GetLuck() { return luck; }

    public int TakeDamage(int damage) { return health -= damage; }

    public int GetHealth() { return health; }
}
