using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHomeEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);

        TargetAttribute1 = Attribute.reaction;
        TargetAttribute2 = Attribute.agility;
        RollAggregate = Aggregate.max;
        DifficultyRating = 1;
        TargetSuccesses = 6;
        MaxFails = 12;
        Message = "\"We have the package, let's hit the road boys!!\"";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }

    public override string MyNameIs()
    {
        Debug.Log("ReturnHomeEvent");
        return "ReturnHomeEvent";
    }
}
