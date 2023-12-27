/*
 * Base abstract class template for more specific events, this handles all actions in a very basic manner but can be overridden
 * 
 * Handles the inner pre- and post- of an event (event start, end)
 * Handles individual stepping of event
 * Maintains event status metadata
 * Handles abstract calculation of the progress of event
 * Provides success/failure state of event
 * Provides provides a way to invoke a twist in the event (traps, environmental)
 * Can invoke a setback
 * Can invoke a breakthrough
 * Handles criticals and critical fails
 * crew status
 * enemy status
 * environmental sub-event
 * morph-out-of and morph-into interface (able to pass information e.g. recon successful, lower difficulty)
 * handle death/dieing (with storyteller assistance)
 * communication with storyteller and antagonist (did a camera capture the crew...)
 * contains non-entity stats (doors HP, terminals)
 * able to be assigned storytelling metadata (what is the events package???, is the chase on foot, in a vehicle (not game relevant but story relevant))
 * ?? handle situations like event state being discovered by antagonist, pass objective information to pro and antagonist
 * 
 * initiative and turn order:
 * interfaces with individual crew and enemy crew members
 * Invokes their step(action) logic
 * 
 * This is where most of the meat and potatoes logic of the "game" will be
 * 
 * GRAPHICS: handles rendering the animation state of a tile (if I end up doing that)
 * 
 * TODO: add cleared event (for event nodes that have been cleared, incase of backtracking)
 * 
 * Event types
 * //Pre-Heist Events
    Pre_InfoGathering,
    Pre_Planning,
    Pre_Navigating,
    Pre_AquirePackage,
    //Heist Events
    Hst_Recon,
    Hst_Sneaking,
    Hst_Masquerading,
    Hst_Hacking,
    Hst_Breaching,
    Hst_Trap,
    Hst_Spotted,
    Hst_HoldHostages,
    //Objective Events
    Obj_StealData,
    Obj_PlantData,
    Obj_StealItem,
    Obj_PlantItem,
    Obj_Kidnap,
    Obj_Extraction,
    Obj_Intimidate,
    Obj_DropOffPackage,
    Obj_DropOffPerson,
    Obj_SearchHostages,
    //Combat Events
    Cmbt_Combat,
    Cmbt_Chase,
    //Post-Heist Events
    Pst_Escape,
    Pst_ReturnHome
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseEvent : MonoBehaviour
{

    [SerializeField]
    private int difficulty;
    [SerializeField]
    private int targetSuccesses;
    [SerializeField]
    private int maxFails;
    [SerializeField]
    private int successes;
    [SerializeField]
    private int fails;
    [SerializeField]
    private int progress;
    [SerializeField]
    private CrewController crew;

    public int Difficulty { get => difficulty; set => difficulty = value; }
    public int NeededSuccesses { get => targetSuccesses; set => targetSuccesses = value; }
    public int MaxFails { get => maxFails; set => maxFails = value; }
    public int Successes { get => successes; set => successes = value; }
    public int Failures { get => fails; set => fails = value; }
    public int Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;
            if (progress > 100) progress = 100;
        }
    }
    public CrewController Crew { get => crew; set => crew = value; }

    public void EventStart(CrewController crew)
    {
        progress = 0;
        successes = 0;
        fails = 0;
        difficulty = Random.Range(8, crew.GetLuck() + ((20 - crew.GetLuck()) / 2));
        targetSuccesses = Random.Range(2, 7);
        MaxFails = Random.Range(5, 10);
        this.crew = crew;
    }

    // TODO: remove - why did I think this was necessary??
    public CrewController GetCrew() { return crew; }

    public void SetProgress(int p){ progress = p; }

    public abstract void EventEnd();

    public abstract bool StepEvent();

    public abstract void MyNameIs();

    public int GetProgress() { return progress; }

    public virtual bool HasSucceeded() { return progress >= 100; }
    public abstract bool HasFailed();

    public void MyBaseNameIs()
    {
        Debug.Log("BaseEvent");
    }
}
