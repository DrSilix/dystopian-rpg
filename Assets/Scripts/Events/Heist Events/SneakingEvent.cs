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
        DifficultyRating = Random.Range(1, 5);
        TargetSuccesses = (5 - DifficultyRating) + 1;
        MaxFails = DifficultyRating + 2;
        int rand = Random.Range(0, 2);
        if (rand == 0) Debug.Log("*A guard is spotted, they'll need to sneak quietly*");
        else Debug.Log("*A camera is watching this hall, stick to the shadows*");
    }

    public override void MyNameIs()
    {
        Debug.Log("SneakingEvent");
    }
}
