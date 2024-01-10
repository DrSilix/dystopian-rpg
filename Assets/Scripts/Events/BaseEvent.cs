/*
 * Base abstract class template for more specific events, this handles all actions in a very basic manner but can be overridden
 * 
 * Handles the inner pre- and post- of an event (event start, end)
 * Handles individual stepping of event
 * Maintains event status metadata
 * Handles abstract calculation of the Progress of event
 * Provides success/failure state of event
 * Provides provides a way to invoke a twist in the event (traps, environmental)
 * Can invoke a setback
 * Can invoke a breakthrough
 * Handles criticals and critical Fails
 * crew status
 * enemy status
 * environmental sub-event
 * morph-out-of and morph-into interface (able to pass information e.g. recon successful, lower Difficulty)
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
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseEvent : MonoBehaviour
{
    
    private int difficultyRating;
    private int maxFails;

    public int DifficultyRating { get => difficultyRating; set { difficultyRating = (value >= 1) ? value : 1; } }
    public int TargetSuccesses { get; set; } = 1;
    public int MaxFails { get => maxFails; set { maxFails = (value >= 1) ? value : 1; } }
    public int Successes { get; set; } = 0;
    public int Fails { get; set; } = 0;
    public int Progress { get; set; } = 0;

    public Attribute TargetAttribute1 { get; set; }
    public Attribute TargetAttribute2 { get; set; }
    public int Modifier { get; set; }
    public Aggregate RollAggregate { get; set; }

    public CrewController Crew { get; set; }
    public CrewController EnemyCrew { get; set; }

    public virtual void EventStart(CrewController crew)
    {
        TargetAttribute1 = Attribute.luck;
        TargetAttribute2 = Attribute.luck;
        Modifier = 0;
        RollAggregate = Aggregate.avg;
        DifficultyRating = Random.Range(1, (crew.GetCrewAttribute(TargetAttribute1) * 2)/4);
        TargetSuccesses = Random.Range(1, 7);
        if (TargetSuccesses == 1) TargetSuccesses = Random.Range(1, 7);
        if (TargetSuccesses > 1) { DifficultyRating -= 2; }
        if (TargetSuccesses > 3) { DifficultyRating -= 1; }
        MaxFails = DifficultyRating;
        if (MaxFails < 3) MaxFails = 3;
        this.Crew = crew;
    }

    public virtual void EventEnd() { }
    
    public virtual bool StepEvent()
    {
        int crewRoll;
        StringBuilder logMessage = new StringBuilder();
        CrewMemberController responsibleCrew;
        (responsibleCrew, crewRoll) = Crew.GetCrewRoll(TargetAttribute1, TargetAttribute2, Modifier, RollAggregate);
        if (responsibleCrew != null) {
            Debug.Log(responsibleCrew.alias + " is taking action! Their stats are " + TargetAttribute1 + ":" + responsibleCrew.GetAttribute(TargetAttribute1) + " and " + TargetAttribute2 + ":" + responsibleCrew.GetAttribute(TargetAttribute2));
            logMessage.Append(responsibleCrew.alias + " acts " + TargetAttribute1.ToString().Substring(0, 3) + ":" + responsibleCrew.GetAttribute(TargetAttribute1) + " and " + TargetAttribute2.ToString().Substring(0, 3) + ":" + responsibleCrew.GetAttribute(TargetAttribute2) + " | ");
        } else
        {
            logMessage.Append("The crew makes an attempt - ");
        }
        Debug.Log(crewRoll + " Successes, DR: " + DifficultyRating +
            " - " + ((crewRoll>=DifficultyRating) ? "SUCCESS!" : "FAILURE!!!"));
        logMessage.Append(crewRoll + " Vs. " + DifficultyRating +
            " - " + ((crewRoll >= DifficultyRating) ? "WIN!" : "LOSS!"));
        if (crewRoll >= DifficultyRating)
        {
            Successes++;
            Debug.Log(" #" + Successes + " out of " + TargetSuccesses);
            logMessage.Append(" " + Successes + "/" + TargetSuccesses);
            GameLog.Instance.PostMessageToLog(logMessage.ToString());
            Progress = Mathf.RoundToInt((float)(Successes * 100) / TargetSuccesses);
            return true;
        }
        Fails++;
        Debug.Log(" #" + Fails + " out of " + MaxFails);
        logMessage.Append(" " + Fails + "/" + MaxFails);
        GameLog.Instance.PostMessageToLog(logMessage.ToString());
        return false;
    }

    public virtual void SubStepOne()
    {

    }

    public virtual void SubStepTwo()
    {

    }

    public virtual void SubStepThree()
    {

    }

    public abstract string MyNameIs();

    public int GetProgress() { return Mathf.RoundToInt(((float)Successes / TargetSuccesses) * 100); }

    public virtual bool HasSucceeded() { return Successes >= TargetSuccesses; }
    public virtual bool HasFailed() { return Fails >= MaxFails; }

    public void MyBaseNameIs()
    {
        Debug.Log("BaseEvent");
    }
}
