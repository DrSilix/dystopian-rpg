using UnityEngine;

public class SneakingEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        TargetAttribute1 = Attribute.agility;
        TargetAttribute2 = Attribute.luck;
        RollAggregate = Aggregate.avg;
        DifficultyRating = Random.Range(3, 7) + 1;
        TargetSuccesses = (9 - DifficultyRating);
        MaxFails = DifficultyRating + 2;
        int rand = Random.Range(0, 2);
        if (rand == 0) Message = "\"A guard is patrolling, keep quiet.\"";
        else Message = "A camera is watching this hall, stick to the shadows";

        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        base.EventStartFollowup();
    }

    public override string MyNameIs()
    {
        Debug.Log("SneakingEvent");
        return "SneakingEvent";
    }
}
