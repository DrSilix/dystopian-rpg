using UnityEngine;

// This is essentially a blank event, used for when a node is revisited and no action by the crew is necessary.
public class CompletedEvent : BaseEvent
{
    public override void EventStart(CrewController crew, HeistLog log)
    {
        base.EventStart(crew, log);
        Message = "This is already completed... replace me, I'm text.";

        Debug.Log(Message);
        GameLog.Instance.PostMessageToLog(Message);
        HeistLogEntry entry = Log.GetCurrent();
        RootLogEntry = entry;
        entry.EntryColor = Color.gray;
        entry.Duration = 5f;
        entry.ShortDescription = Message;
    }

    public override bool HasSucceeded() { return true; }
    public override bool HasFailed() { return false; }

    public override string MyNameIs()
    {
        Debug.Log("CompletedEvent");
        return "CompletedEvent";
    }
}
