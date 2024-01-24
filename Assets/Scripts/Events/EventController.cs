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

/// <summary>
/// Handles overall administration of an individual event. requires an event type along with event script component for that type.
/// Is attached to a physical node in the game world
/// Also handles taking in a player crew and passing that crew along to the next event at the next node.
/// </summary>
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

    /// <summary>
    /// Attaches an event to the parent game object given an Heist Event Type
    /// </summary>
    /// <param name="eventType">Heist Event Type to use here</param>
    public void AssociateEvent(HEventType.HType eventType)
    {
        this.eventType = eventType;
        BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        eventState = HEventState.IdleUnfinished;
    }

    /// <summary>
    /// Transforms the current event to a different (or I guess same) event type, resets progress and restarts
    /// </summary>
    /// <param name="eventType">The event type to mutate into</param>
    public void MutateEvent(HEventType.HType eventType)
    {
        Destroy(BaseEvent);
        this.eventType = eventType;
        BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        BaseEvent.EnemyCrew = possesedEnemies;
        eventState = HEventState.IdleUnfinished;
        Invoke(nameof(BeginHeistEvent), 3f);
    }

    /// <summary>
    /// Used to update the state of the event (unfinished, running, finished, ...) and notifying the storyteller
    /// Notify's the storyteller even if the state is "changed" to the same state
    /// (this can be used as a sort of game loop)
    /// </summary>
    /// <param name="state">Heist Event State to change to</param>
    public void ChangeHeistEventState(HEventState state)
    {
        //if (eventState == state) return; //I don't actually want to do this, Event though the state isn't changing the properties are
        eventState = state;
        Storyteller.Instance.heistEventStateChanged.Invoke(this);
    }
    
    public BaseEvent GetBaseEvent() { return BaseEvent; }
    public HEventType.HType GetEventType() { return eventType; }
    public HEventState GetEventState() {  return eventState; }

    /// <summary>
    /// Associates an enemy crew with this eventcontroller, to presumably be used
    /// for combat events (or regular events in the future)
    /// </summary>
    /// <param name="enemyCrew">CrewController to associate as enemies</param>
    public void EnemyIntake(CrewController enemyCrew)
    {
        possesedEnemies = enemyCrew;
    }

    /// <summary>
    /// Associates a crew with this EventController
    /// </summary>
    /// <param name="crew"></param>
    public void CrewIntake(CrewController crew)
    {
        Debug.Log("Taking in Crew");
        possesedCrew = crew;
        crew.transform.position = this.gameObject.transform.position;
        node.SetColor(Color.cyan);
    }

    /// <summary>
    /// Works in tandem with MovePlayerCrew to physically (in game world) move the player crew to the next node
    /// It also invokes asyncronously but simultaneously to the physical movement passing the crew to the next node
    /// </summary>
    public void TransportCrewToNextNode()
    {
        Debug.Log("Crew Moving To Next Node");
        possesedCrew.GetComponentInParent<MovePlayerCrew>().MoveTo(node.GetDownstreamNode().transform.position, 3.0f * node.GetLineLength());
        Invoke(nameof(CrewPassToNext), 3.0f * node.GetLineLength());
    }

    /// <summary>
    /// Hands off the crew to the next node, disables this game object, and starts the next node
    /// </summary>
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

    /// <summary>
    /// Primarily performs the events (BaseEvent) EventStart method and then invokes the loop on a delay
    /// </summary>
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

    /// <summary>
    /// Main game loop, this steps the event and relies on the invariant that the event will
    /// eventually return true for HasSucceeded or HasFailed. Invokes next step on a delay
    /// Once success or failure, calls to end the event
    /// </summary>
    public void HeistEventLoop()
    {
        ChangeHeistEventState(HEventState.Running);
        BaseEvent.StepEvent();
        //Debug.Log($"Event Progress: {baseEvent.GetProgress()}%");
        if (BaseEvent.HasSucceeded() || BaseEvent.HasFailed()) { EndHeistEvent(); }
        else { Invoke(nameof(HeistEventLoop), StepDelayTime);  }
    }

    /// <summary>
    /// Cleans up the event by cancelling any open invokes, calling the EventEnd method
    /// And then reacting to whether the event succeeded or failed
    /// </summary>
    public void EndHeistEvent()
    {
        Debug.Log("End Heist Event");
        // skyscraperBG.SetActive(false);
        CancelInvoke();
        BaseEvent.EventEnd();
        // The rest of this could possibly be put inside the base event
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
