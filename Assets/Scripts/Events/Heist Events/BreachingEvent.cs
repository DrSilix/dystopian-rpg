using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreachingEvent : BaseEvent
{
    public override void EventStart(CrewController crew)
    {
        base.EventStart(crew);
        TargetAttribute1 = Attribute.body;
        TargetAttribute2 = Attribute.strength;
        RollAggregate = Aggregate.max;
        DifficultyRating = Random.Range(1, 3);
        TargetSuccesses = DifficultyRating;
        MaxFails = DifficultyRating+2;

        string msg = "\"Maybe if I can spike into the finger print sensor I can ... \" \"Step aside I'll clober this door off it's hinges!\" \"Won't that attract attention .. \" *SMASH*";
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }
    public override void MyNameIs()
    {
        Debug.Log("BreachingEvent");
    }
}
