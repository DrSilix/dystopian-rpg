using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakingEvent : BaseEvent
{
    public override void EventStart(CrewController crew)
    {
        base.EventStart(crew);
        TargetAttribute1 = Attribute.agility;
        TargetAttribute2 = Attribute.luck;
        RollAggregate = Aggregate.avg;
        DifficultyRating = Random.Range(3, 7) + 1;
        TargetSuccesses = (9 - DifficultyRating);
        MaxFails = DifficultyRating + 2;
        int rand = Random.Range(0, 2);
        string msg;
        if (rand == 0) msg = "\"A guard is patrolling, keep quiet.\"";
        else msg = "A camera is watching this hall, stick to the shadows";

        Debug.Log(msg);
        GameLog.Instance.PostMessageToLog(msg);
    }

    public override string MyNameIs()
    {
        Debug.Log("SneakingEvent");
        return "SneakingEvent";
    }
}
