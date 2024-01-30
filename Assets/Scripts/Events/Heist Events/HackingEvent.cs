using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        TargetAttribute1 = Attribute.logic;
        TargetAttribute2 = Attribute.logic;
        RollAggregate = Aggregate.max;
        DifficultyRating = Random.Range(4,7) + 1;
        TargetSuccesses = (10 - DifficultyRating) + 1;
        MaxFails = DifficultyRating + 2;
        int rand = Random.Range(0, 2);
        if (rand == 0) Message = "\"Maybe if I can spike into and recombobulate the door pad, I can jam it open...\"";
        else Message = "\"We can't sneak past this " + ((Random.Range(0,2) == 1) ? "trip wire" : "motion sensor") + " I'll have to spike in and zap it\"";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }

    public override string MyNameIs()
    {
        Debug.Log("HackingEvent");
        return "HackingEvent";
    }
}
