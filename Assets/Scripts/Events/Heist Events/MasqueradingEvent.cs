using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasqueradingEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        TargetAttribute1 = Attribute.charisma;
        TargetAttribute2 = Attribute.intuition;
        RollAggregate = Aggregate.avg;
        DifficultyRating = Random.Range(3, 5) + 1;
        TargetSuccesses = (7 - DifficultyRating) + 1;
        MaxFails = DifficultyRating + 2;
        Message = "\"Some worker bees drudging along. Act natural boys we'll slip right by\"";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }

    public override string MyNameIs()
    {
        Debug.Log("MasqueradingEvent");
        return "MasqueradingEvent";
    }
}
