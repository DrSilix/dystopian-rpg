/*
 * Stores the heist event and calls basic generic functions on a clock to simulate turns and progress
 * 
 * associate an event of a specific type with a node
 * handles beginning an event and ending
 * Handles event clock, stepping the event at interval
 * fascilitates the transport of a "crew" (immigration, travelling to, and emmigration)
 * NO > Make decisions about a failed event, morph to a new event, start extraction, generate an alternate path .. storyteller handles this
 * TODO: Contains subscribable events to gain status updates (storyteller)
 * TODO: triggers success and failure of a heist event (subscribed event, storyteller)
 * NO > Able to handle resolution actions (distribute loot, xp, etc.) ... the storytell will handle this
 * way to handle a redirected crew transfer (alternate evac) currently transfer is from>to (generalize transfer to)
 * holding place for enemies
 * 
 * I think i've decided the event controller should be encapsulated/isolated. It doesn't need to know
 * what other events are doing, it just puts it's head down and trudges along on it's one job.
 * 
 * The other idea I had was to have an eventcontrollermanager (lol) that handled all events, but I think the
 * storyteller/protagonist will fill that role. It's better that one thing does one thing really good
 * 
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField]
    private HEventType.HType eventType;
    [SerializeField]
    private CrewController possesedCrew;
    [SerializeField]
    private CrewController possesedEnemies;
    [SerializeField]
    private HEventState eventState;

    public float StepDelayTime = 3f;

    public BaseEvent BaseEvent { get; private set; }
    
    public Node node;

    public void AssociateEvent(HEventType.HType eventType)
    {
        this.eventType = eventType;
        BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        eventState = HEventState.IdleUnfinished;
    }

    public void MutateEvent(HEventType.HType eventType)
    {
        Destroy(BaseEvent);
        this.eventType = eventType;
        BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        BaseEvent.EnemyCrew = possesedEnemies;
        eventState = HEventState.IdleUnfinished;
        Invoke(nameof(BeginHeistEvent), 3f);
    }

    public void ChangeHeistEventState(HEventState state)
    {
        //if (eventState == state) return; //I don't actually want to do this, Event though the state isn't changing the properties are
        eventState = state;
        Storyteller.Instance.heistEventStateChanged.Invoke(this);
    }

    public BaseEvent GetBaseEvent() { return BaseEvent; }
    public HEventType.HType GetEventType() { return eventType; }
    public HEventState GetEventState() {  return eventState; }

    public void EnemyIntake(CrewController enemyCrew)
    {
        possesedEnemies = enemyCrew;
    }

    public void CrewIntake(CrewController crew)
    {
        Debug.Log("Taking in Crew");
        possesedCrew = crew;
        crew.transform.position = this.gameObject.transform.position;
        node.SetColor(Color.cyan);
    }

    public void TransportCrewToNextNode()
    {
        Debug.Log("Crew Moving To Next Node");
        possesedCrew.GetComponentInParent<MovePlayerCrew>().MoveTo(node.GetDownstreamNode().transform.position, 3.0f * node.GetLineLength());
        Invoke(nameof(CrewPassToNext), 3.0f * node.GetLineLength());
    }

    public void CrewPassToNext()
    {
        Debug.Log("Crew Moved To Next Node");
        EventController nextNode = node.GetDownstreamNode().eventController;
        nextNode.CrewIntake(possesedCrew);
        possesedCrew = null;
        nextNode.enabled = true;
        this.enabled = false;
        nextNode.BeginHeistEvent();
    }

    public void BeginHeistEvent()
    {
        ChangeHeistEventState(HEventState.Begin);
        Debug.Log("-----------------------");
        Debug.Log("Begin Heist Event");
        // skyscraperBG = Camera.main.transform.GetChild(1).gameObject;
        // skyscraperBG.SetActive(true);
        BaseEvent.EventStart(possesedCrew);
        BaseEvent.MyNameIs();
        Debug.Log($"Event Progress: {BaseEvent.GetProgress()}%");
        Invoke(nameof(HeistEventLoop), 7f);
    }

    public void HeistEventLoop()
    {
        ChangeHeistEventState(HEventState.Running);
        BaseEvent.StepEvent();
        //Debug.Log($"Event Progress: {baseEvent.GetProgress()}%");
        if (BaseEvent.HasSucceeded() || BaseEvent.HasFailed()) { EndHeistEvent(); }
        else { Invoke(nameof(HeistEventLoop), StepDelayTime);  }
    }


    public void EndHeistEvent()
    {
        Debug.Log("End Heist Event");
        // skyscraperBG.SetActive(false);
        CancelInvoke();
        BaseEvent.EventEnd();
        if (BaseEvent.HasFailed())
        {
            // TODO: FIX event node with no enemies, ?get lucky? phew
            if (eventType != HEventType.HType.Cmbt_Combat)
            {
                Debug.Log("Enemy Spotted!!");
                GameLog.Instance.PostMessageToLog("You've been spotted! Get ready for combat!");
                ChangeHeistEventState(HEventState.DoneFailure);
                node.SetColor(Color.red);
                MutateEvent(HEventType.HType.Cmbt_Combat);
                //Debug.Log("Curiously the mutated event has ended now.");
                return;
            }
            Debug.Log("Heist FAILED!!");
            GameLog.Instance.PostMessageToLog("You're crew is dead or captured. Failed!");
            ChangeHeistEventState(HEventState.DoneFailure);
            node.SetColor(Color.red);
            return;
        }
        if (node.GetDownstreamNode() == null)
        {
            Debug.Log("Finished Heist");
            node.SetColor(Color.green);
            GameLog.Instance.PostMessageToLog("Finished Heist");
            ChangeHeistEventState(HEventState.DoneSuccess);
            return;
        }
        ChangeHeistEventState(HEventState.DoneSuccess);
        node.SetColor(Color.grey);
        Invoke(nameof(TransportCrewToNextNode), 2f);
    }
}
