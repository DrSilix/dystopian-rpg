using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealDataEvent : BaseEvent
{
    private int roundNumber;
    private int randomEventRound;
    public override void EventStart(CrewController crew)
    {
        this.Crew = crew;
        Modifier = 0;
        TargetSuccesses = Random.Range(5, 12);
        MaxFails = TargetSuccesses + 8;
        DifficultyRating = 2;
        roundNumber = 0;
        randomEventRound = Random.Range(12, 18);
        string msg = "\"Finally!! Get the data smarty pants and let's jolt\" \"Alright I'm on it but it's going to take me about 10 minutes. Big guy stand watch over there. Slick can you help me with this\"";
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }

    public override bool StepEvent()
    {
        roundNumber++;
        // hacker attempts to steal data
        // face assists
        // enforcer keeps watch
        CrewMemberController hacker = Crew.GetCrewMember(2);
        CrewMemberController face = Crew.GetCrewMember(3);
        CrewMemberController enforcer = Crew.GetCrewMember(1);
        
        
        (int faceRoll, int faceCrit) = face.GetAttributeAdvancedRoll(Attribute.logic, Attribute.charisma, 0);
        Debug.Log("Face rolls " + faceRoll + " successes to help the hacker" + CriticalMessage(faceCrit));
        GameLog.Instance.PostMessageToLog("Face rolls " + faceRoll + " successes to help the hacker" + CriticalMessage(faceCrit));
        if (faceCrit == 1) faceRoll += faceRoll;
        if (faceCrit == -1) { Successes--; faceRoll = -1; }
        (int hackerRoll, int hackerCrit) = hacker.GetAttributeAdvancedRoll(Attribute.logic, Attribute.logic, faceRoll);
        (int enforceRoll, int enforceCrit) = enforcer.GetAttributeAdvancedRoll(Attribute.intuition, Attribute.luck, 2);
        Debug.Log("Enforcer keeps watch with " + enforceRoll + " successes - \"Everythings clear, no one in sight.\"");
        GameLog.Instance.PostMessageToLog("Enforcer keeps watch with " + enforceRoll + " successes - \"Everythings clear, no one in sight.\"");

        if (roundNumber == randomEventRound && enforceRoll < DifficultyRating + 1)
        {
            Debug.Log("\"INTRUDER!!! OVER THERE!\" \"Shit! we've been spotted, let's jolt!\"");
            GameLog.Instance.PostMessageToLog("\"INTRUDER!!! OVER THERE!\" \"Shit! we've been spotted, let's jolt!\"");
            Fails = MaxFails;
            return false;
        }

        string msg = "With the Face's help the hacker rolls " + hackerRoll + " DR: " + DifficultyRating +
            " - " + ((hackerRoll >= DifficultyRating) ? "SUCCESS!" : "FAILURE!!!") + CriticalMessage(hackerRoll);
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
        if (hackerRoll >= DifficultyRating)
        {
            Successes++;
            if (hackerCrit == 1) { Successes++; }
            Debug.Log("\"Okay " + Successes + " down " + (TargetSuccesses - Successes) + " to go.\"");
            GameLog.Instance.PostMessageToLog("\"Okay " + Successes + " down " + (TargetSuccesses - Successes) + " to go.\"");
            Debug.Log("Success #" + Successes + " out of " + TargetSuccesses);
            Progress = Mathf.RoundToInt((float)(Successes * 100) / TargetSuccesses);
            return true;
        }
        Fails++;
        Debug.Log("\"Shit!...\"");
        GameLog.Instance.PostMessageToLog("\"Shit!... I've only got " + Fails + " out of " + MaxFails + " tries left before the alarms trigger");
        Debug.Log("FAILURE! #" + Fails + " out of " + MaxFails);
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
        if (crit == -1) return " - CRITICAL FAILURE!!!!!!";
        return "";
    }

    public override string MyNameIs()
    {
        Debug.Log("StealDataEvent");
        return "StealDataEvent";
    }
}
