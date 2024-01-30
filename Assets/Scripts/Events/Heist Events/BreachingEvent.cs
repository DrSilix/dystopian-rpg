using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreachingEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        TargetAttribute1 = Attribute.body;
        TargetAttribute2 = Attribute.strength;
        RollAggregate = Aggregate.max;
        DifficultyRating = Random.Range(2, 6) + 1;
        TargetSuccesses = DifficultyRating;
        MaxFails = DifficultyRating+2;

        Message = "\"Step aside I'll clober this door off it's hinges!\" *SMASH*";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }
    public override string MyNameIs()
    {
        Debug.Log("BreachingEvent");
        return "BreachingEvent";
    }
}
