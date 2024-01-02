/*
 * Architect of all, persists accross individual heists
 * Maintains awareness of all game mechanic states (events, crew, enemies, etc.)
 * Contains story information (written and abstract(checks on how the antagonist reacts)) and handles invoking that in story scenes and overarching actions (a guiding hand to the protagonist)
 * 
 * is this the protagonist director?
 * or does it control the protagonist and antagonist directors?
 * 
 * 
 * player director (watches over players and decides next steps on paradigm change)
 * enemy director (oblivious, gets fed events (alarms, etc.) triggers reactions)
 * 
 * TODO: make storyteller!
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Storyteller : MonoBehaviour
{
    [SerializeField] private GameObject crewPrefab;
    [SerializeField] private GameObject crewMember1Prefab, crewMember2Prefab, crewMember3Prefab;
    
    
    public CrewController Crew {  get; private set; }
    
    
    public static Storyteller Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        InitializeInstance();
    }

    private void InitializeInstance()
    {
        GenerateCrew();
    }

    private void GenerateCrew()
    {
        GameObject crewGO = Instantiate(crewPrefab, Vector3.zero, Quaternion.identity);
        Crew = crewGO.GetComponent<CrewController>();
        GameObject crewMember1 = Instantiate(crewMember1Prefab, crewGO.transform);
        GameObject crewMember2 = Instantiate(crewMember2Prefab, crewGO.transform);
        GameObject crewMember3 = Instantiate(crewMember3Prefab, crewGO.transform);
        Crew.AddCrewMembers(crewMember1, crewMember2, crewMember3);
    }
}
