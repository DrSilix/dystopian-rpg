using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavigationEvent : BaseEvent
{
    private GameObject background;
    public override void EventStart(CrewController crew)
    {
        background = Camera.main.transform.GetChild(0).gameObject;
        background.SetActive(true);
        
        base.EventStart(crew);
        TargetAttribute1 = Attribute.reaction;
        TargetAttribute2 = Attribute.agility;
        RollAggregate = Aggregate.max;
        DifficultyRating = 1;
        TargetSuccesses = 6;
        MaxFails = 16;
        Debug.Log("\"We have the destination in navigation, let's hit the road boys!!\"");
        GameLog.Instance.PostMessageToLog("\"We have the destination in navigation, let's hit the road boys!!\"");
    }

    public override void MyNameIs()
    {
        Debug.Log("NavigationEvent");
    }

    public override void EventEnd()
    {
        base.EventEnd();
        Debug.Log("\"We've arrived at Texico plant. We'll go in, brains here will steal the who-sa-ma-what-sit, and we'll get out. Quick and clean\"");
        GameLog.Instance.PostMessageToLog("\"We've arrived at Texico plant. We'll go in, brains here will steal the who-sa-ma-what-sit, and we'll get out. Quick and clean\"");
        background.SetActive(false);
    }
}
