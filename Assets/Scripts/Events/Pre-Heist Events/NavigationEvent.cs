using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavigationEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        TargetAttribute1 = Attribute.reaction;
        TargetAttribute2 = Attribute.agility;
        RollAggregate = Aggregate.max;
        DifficultyRating = 1;
        TargetSuccesses = 6;
        MaxFails = 16;
        Message = "\"We have the destination in navigation, let's hit the road boys!!\"";
        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }

    public override string MyNameIs()
    {
        Debug.Log("NavigationEvent");
        return "NavigationEvent";
    }

    public override void EventEnd()
    {
        base.EventEnd();
        Debug.Log("\"We've arrived at Texico plant. We'll go in, brains here will steal the who-sa-ma-what-sit, and we'll get out. Quick and clean\"");
        GameLog.Instance.PostMessageToLog("\"We've arrived at Texico plant. We'll go in, brains here will steal the who-sa-ma-what-sit, and we'll get out. Quick and clean\"");
        Log.GetCurrent().Body = "\"We've arrived at Texico plant. We'll go in, brains here will steal the who-sa-ma-what-sit, and we'll get out. Quick and clean\"";
    }
}
