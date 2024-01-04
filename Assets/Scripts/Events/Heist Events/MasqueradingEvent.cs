using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasqueradingEvent : BaseEvent
{
    public override void EventStart(CrewController crew)
    {
        base.EventStart(crew);
        TargetAttribute1 = Attribute.charisma;
        TargetAttribute2 = Attribute.intuition;
        RollAggregate = Aggregate.avg;
        DifficultyRating = Random.Range(1, 4);
        TargetSuccesses = (3 - DifficultyRating) + 1;
        MaxFails = DifficultyRating + 3;
        string msg = "\"Some worker bees drudging along. Act natural boys we'll slip right by\"";
        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }

    public override string MyNameIs()
    {
        Debug.Log("MasqueradingEvent");
        return "MasqueradingEvent";
    }
}
