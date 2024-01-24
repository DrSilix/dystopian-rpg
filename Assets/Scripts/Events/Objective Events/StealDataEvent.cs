using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an objective event and a case study for a more complex event
/// </summary>
public class StealDataEvent : BaseEvent
{
    public enum StepNumber
    {
        One,
        Two,
        Three
    }
    
    private int roundNumber;
    private int randomEventRound;

    private int faceRoll, faceCrit;
    private int enforcerRoll;
    //private int hackerRoll, hackerCrit;

    private StepNumber stepNumber;

    private CrewMemberController hacker;
    private CrewMemberController face;
    private CrewMemberController enforcer;
    public override void EventStart(CrewController crew)
    {
        this.Crew = crew;

        hacker = Crew.GetCrewMember(1);
        face = Crew.GetCrewMember(2);
        enforcer = Crew.GetCrewMember(0);

        Modifier = 0;
        TargetSuccesses = Random.Range(5, 12);
        MaxFails = TargetSuccesses + 8;
        DifficultyRating = 3;
        roundNumber = 0;
        stepNumber = StepNumber.One;
        // used for the person keeping watch, they are only ever able to fail on this round
        randomEventRound = Random.Range(3, TargetSuccesses);
        string msg = "\"Finally!! Get the data smarty pants and let's jolt\" \"Alright I'm on it but it's going to take me about 10 minutes. Big guy stand watch over there. Slick can you help me with this\"";
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }

    public override bool StepEvent()
    {
        // instead of using the step one, two, and three methods I ended up (after completing the combat event) using a switch/case state machine
        // to create a sub-step for events. (a sub-step I'm defining as a step that does not result in a success or failure but is a sub part of a full step)
        switch (stepNumber)
        {
            case StepNumber.One:
                roundNumber++;
                (faceRoll, faceCrit) = face.GetAttributeAdvancedRoll(Attribute.logic, Attribute.charisma, 0);
                Debug.Log($"{face.alias} rolls {faceRoll} successes to help {hacker.alias}{CriticalMessage(faceCrit)}");
                GameLog.Instance.PostMessageToLog($"{face.alias} rolls {faceRoll} successes to help {hacker.alias} find the data {CriticalMessage(faceCrit)}");
                if (faceCrit == 1) faceRoll += faceRoll;
                if (faceCrit == -1) { Successes--; faceRoll = -1; }
                stepNumber = StepNumber.Two;
                return false;
            case StepNumber.Two:
                (enforcerRoll, _) = enforcer.GetAttributeAdvancedRoll(Attribute.intuition, Attribute.luck, 2);
                Debug.Log($"{enforcer.alias} keeps watch with {enforcerRoll} successes - \"Everythings clear, no one in sight.\"");
                GameLog.Instance.PostMessageToLog($"{enforcer.alias} keeps watch with {enforcerRoll} successes - \"Everythings clear, no one in sight.\"");

                if (roundNumber == randomEventRound && enforcerRoll< 7)
                {
                    Debug.Log("\"INTRUDER!!! OVER THERE!\" \"Shit! we've been spotted, let's jolt!\"");
                    GameLog.Instance.PostMessageToLog("\"INTRUDER!!! OVER THERE!\" \"Shit! we've been spotted, let's jolt!\"");
                    Fails = MaxFails;
                    return false;
                }
                stepNumber = StepNumber.Three;
                return false;
            case StepNumber.Three:
                (int hackerRoll, int hackerCrit) = hacker.GetAttributeAdvancedRoll(Attribute.logic, Attribute.logic, faceRoll);
                string msg = $"With the {face.alias}'s help {hacker.alias} rolls {hackerRoll} DR: {DifficultyRating} - {((hackerRoll >= DifficultyRating) ? "SUCCESS!" : "FAILURE!!!")}{CriticalMessage(hackerRoll)}";
                Debug.Log(msg);
                GameLog.Instance.PostMessageToLog(msg);

                // state machine must be switched before any returns
                stepNumber = StepNumber.One;

                if (hackerRoll >= DifficultyRating)
                {
                    Successes++;
                    if (hackerCrit == 1) { Successes++; }
                    Debug.Log($"\"Okay {Successes} down {TargetSuccesses - Successes} to go.\"");
                    GameLog.Instance.PostMessageToLog($"\"Okay {Successes} down {TargetSuccesses - Successes} to go.\"");
                    Debug.Log($"Success #{Successes} out of {TargetSuccesses}");
                    Progress = Mathf.RoundToInt((float)(Successes * 100) / TargetSuccesses);
                    return true;
                }
                Fails++;
                Debug.Log("\"Shit!...\"");
                GameLog.Instance.PostMessageToLog($"\"Shit!... I've only got {Fails} out of {MaxFails} tries left before the alarms trigger");
                Debug.Log($"FAILURE! #{Fails} out of {MaxFails}");
                return false;
        }
        return false;
    }

    public override void EventEnd()
    {
        base.EventEnd();
        Debug.Log("\"GOT IT!! Now let's jolt! Slick lead the way out!\"");
        GameLog.Instance.PostMessageToLog("\"GOT IT!! Now let's jolt! Slick lead the way out!\"");
    }

    private string CriticalMessage(int crit)
    {
        if (crit == 1) return " - CRITICAL SUCCESS!!!";
        if (crit == -1) return " - CRITICAL FAILURE!!!";
        return "";
    }

    public override string MyNameIs()
    {
        Debug.Log("StealDataEvent");
        return "StealDataEvent";
    }
}
