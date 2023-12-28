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
        Debug.Log("\"Some worker bees drudging along. Act natural boys we'll slip right by\"");
    }

    public override void MyNameIs()
    {
        Debug.Log("MasqueradingEvent");
    }
}
