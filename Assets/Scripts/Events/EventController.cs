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

    private BaseEvent baseEvent;
    
    public Node node;

    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssociateEvent(HEventType.HType eventType)
    {
        this.eventType = eventType;
        baseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
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
        Debug.Log("Begin Heist Event");
        baseEvent.EventStart(possesedCrew);
        baseEvent.MyNameIs();
        Debug.Log("Event Progress: " + baseEvent.GetProgress() + "%");
        Invoke("HeistEventLoop", 2f);
    }

    public void HeistEventLoop()
    {
        baseEvent.StepEvent();
        Debug.Log("Event Progress: " + baseEvent.GetProgress() + "%");
        if (baseEvent.HasSucceeded() || baseEvent.HasFailed()) { EndHeistEvent(); }
        else { Invoke("HeistEventLoop", 2f);  }
    }


    public void EndHeistEvent()
    {
        Debug.Log("End Heist Event");
        CancelInvoke();
        if (baseEvent.HasFailed())
        {
            Debug.Log("Heist FAILED!!");
            node.SetColor(Color.red);
            return;
        }
        if (node.GetDownstreamNode() == null)
        {
            Debug.Log("Finished Heist");
            node.SetColor(Color.green);
            return;
        }
        node.SetColor(Color.grey);
        TransportCrewToNextNode();
    }
}
