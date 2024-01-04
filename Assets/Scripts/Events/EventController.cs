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
 * TODO: holding place for enemies
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
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField]
    private HEventType.HType eventType;
    [SerializeField]
    private CrewController possesedCrew;
    [SerializeField]
    private HEventState eventState;

    private BaseEvent baseEvent;
    
    public Node node;

    public void AssociateEvent(HEventType.HType eventType)
    {
        this.eventType = eventType;
        baseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        eventState = HEventState.IdleUnfinished;
    }

    public void ChangeHeistEventState(HEventState state)
    {
        //if (eventState == state) return; //I don't actually want to do this, Event though the state isn't changing the properties are
        eventState = state;
        Storyteller.Instance.heistEventStateChanged.Invoke(this);
    }

    public BaseEvent GetBaseEvent() { return baseEvent; }
    public HEventType.HType GetEventType() { return eventType; }
    public HEventState GetEventState() {  return eventState; }

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
        possesedCrew.moveTo(node.GetDownstreamNode().transform.position, 3.0f * node.GetLineLength());
        Invoke("CrewPassToNext", 3.0f * node.GetLineLength());
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
        baseEvent.EventStart(possesedCrew);
        baseEvent.MyNameIs();
        Debug.Log("Event Progress: " + baseEvent.GetProgress() + "%");
        Invoke("HeistEventLoop", 7f);
    }

    public void HeistEventLoop()
    {
        ChangeHeistEventState(HEventState.Running);
        baseEvent.StepEvent();
        Debug.Log("Event Progress: " + baseEvent.GetProgress() + "%");
        if (baseEvent.HasSucceeded() || baseEvent.HasFailed()) { EndHeistEvent(); }
        else { Invoke("HeistEventLoop", 3f);  }
    }


    public void EndHeistEvent()
    {
        Debug.Log("End Heist Event");
        // skyscraperBG.SetActive(false);
        CancelInvoke();
        baseEvent.EventEnd();
        if (baseEvent.HasFailed())
        {
            Debug.Log("Heist FAILED!!");
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
        Invoke("TransportCrewToNextNode", 2f);
    }
}
