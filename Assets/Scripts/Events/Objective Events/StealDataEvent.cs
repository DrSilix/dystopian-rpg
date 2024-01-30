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
    public override void EventStart(CrewController crew, HeistLog log)
    {
        this.Crew = crew;
        Log = log;

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
        Message = "\"Finally!! Get the data smarty pants and let's jolt\" \"Alright I'm on it but it's going to take me about 10 minutes. Big guy stand watch over there. Slick can you help me with this\"";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);

        HeistLogEntry entry = Log.GetCurrent();
        RootLogEntry = entry;
        entry.EntryColor = Color.blue;
        entry.Duration = 7f;
        entry.ShortDescription = base.Message;
        List<CrewMemberController> crewMembers = Crew.CrewMembers;
        entry.Body = $"{crewMembers[0].alias} keeps watch:\t{crewMembers[0].GetAttribute(Attribute.intuition)} + {crewMembers[0].GetAttribute(Attribute.luck)}\n" +
                $"{crewMembers[1].alias} hacks in:\t{crewMembers[1].GetAttribute(Attribute.logic)} + {crewMembers[1].GetAttribute(Attribute.logic)}\n" +
                $"{crewMembers[2].alias} helps {crewMembers[1].alias}:\t{crewMembers[2].GetAttribute(Attribute.logic)} + {crewMembers[2].GetAttribute(Attribute.charisma)}";
    }

    public override bool StepEvent()
    {
        HeistLogEntry entry = Log.GetCurrent();

        // instead of using the step one, two, and three methods I ended up (after completing the combat event) using a switch/case state machine
        // to create a sub-step for events. (a sub-step I'm defining as a step that does not result in a success or failure but is a sub part of a full step)
        switch (stepNumber)
        {
            case StepNumber.One:
                roundNumber++;
                (faceRoll, faceCrit) = face.GetAttributeAdvancedRoll(Attribute.logic, Attribute.charisma, 0);
                if (faceCrit == 1) faceRoll += faceRoll;
                if (faceCrit == -1) { Successes--; faceRoll = -1; }
                Message = $"{face.alias} rolls {faceRoll} successes to help {hacker.alias} find the data {CriticalMessage(faceCrit)}";
                entry.Duration = 10f;
                stepNumber = StepNumber.Two;
                break;
            case StepNumber.Two:
                (enforcerRoll, _) = enforcer.GetAttributeAdvancedRoll(Attribute.intuition, Attribute.luck, 2);
                if (roundNumber == randomEventRound && enforcerRoll< 7)
                {
                    Message = "\"INTRUDER!!! OVER THERE!\" \"Shit! we've been spotted, let's jolt!\"";
                    Fails = MaxFails;
                    break;
                }
                Message = $"{enforcer.alias} keeps watch with {enforcerRoll} successes - \"Everythings clear, no one in sight.\"";
                entry.Duration = 10f;
                stepNumber = StepNumber.Three;
                break;
            case StepNumber.Three:
                (int hackerRoll, int hackerCrit) = hacker.GetAttributeAdvancedRoll(Attribute.logic, Attribute.logic, faceRoll);
                Message = $"With the {face.alias}'s help {hacker.alias} rolls {hackerRoll} DR:{DifficultyRating} - {((hackerRoll >= DifficultyRating) ? "WIN!" : "LOSS!")}{CriticalMessage(hackerCrit)}";

                // state machine must be switched before any returns
                stepNumber = StepNumber.One;

                if (hackerRoll >= DifficultyRating)
                {
                    Successes++;
                    if (hackerCrit == 1) { Successes++; }
                    Message += $" {Successes}/{TargetSuccesses}";
                    Progress = Mathf.RoundToInt((float)(Successes * 100) / TargetSuccesses);
                    entry.Duration = 10f;
                    entry.EntryColor = Color.green;
                }
                else
                {
                    Fails++;
                    Message += $" {Fails}/{MaxFails}";
                    entry.Duration = 30f;
                    entry.EntryColor = Color.red;

                }
                break;
        }

        entry.ShortDescription = Message;
        entry.ParentEntry = RootLogEntry;

        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
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
