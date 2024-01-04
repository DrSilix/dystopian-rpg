using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingEvent : BaseEvent
{
    public override void EventStart(CrewController crew)
    {
        base.EventStart(crew);
        TargetAttribute1 = Attribute.logic;
        TargetAttribute2 = Attribute.logic;
        RollAggregate = Aggregate.max;
        DifficultyRating = Random.Range(2,5);
        TargetSuccesses = (5 - DifficultyRating) + 1;
        MaxFails = DifficultyRating + 2;
        int rand = Random.Range(0, 2);
        string msg;
        if (rand == 0) msg = "\"Maybe if I can spike into and recombobulate the door pad, I can jam it open...\"";
        else msg = "\"We can't sneak past this " + ((Random.Range(0,2) == 1) ? "trip wire" : "motion sensor") + " I'll have to spike in and zap it\"";
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }

    public override string MyNameIs()
    {
        Debug.Log("HackingEvent");
        return "HackingEvent";
    }
}
