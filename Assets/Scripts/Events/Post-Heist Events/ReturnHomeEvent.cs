using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHomeEvent : BaseEvent
{
    private GameObject background;
    public override void EventStart(CrewController crew)
    {
        base.EventStart(crew);
        background = Camera.main.transform.GetChild(0).gameObject;
        background.SetActive(true);

        TargetAttribute1 = Attribute.reaction;
        TargetAttribute2 = Attribute.agility;
        RollAggregate = Aggregate.max;
        DifficultyRating = 1;
        TargetSuccesses = 6;
        MaxFails = 12;
        Debug.Log("\"We have the package, let's hit the road boys!!\"");
        GameLog.Instance.PostMessageToLog("\"We have the package, let's hit the road boys!!\"");
    }

    public override void MyNameIs()
    {
        Debug.Log("ReturnHomeEvent");
    }
}
