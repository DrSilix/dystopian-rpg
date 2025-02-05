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

using System;
using UnityEngine;

/// <summary>
/// Handles overall administration of an individual event.
/// NO LONGER THE CASE (requires an event type along with event script component for that type.)
/// Expects associateEvent be called before manipulating the object
/// NO LONGER THE CASE (Is attached to a physical node in the game world)
/// Also handles taking in a player crew -object instance- and passing that crew along to the next event at the next node.
/// </summary>
public class EventController
{
    [SerializeField]
    private HEventType.HType eventType;
    [SerializeField]
    private CrewController possesedCrew;
    [SerializeField]
    private CrewController possesedEnemies;
    [SerializeField]
    private HEventState eventState;

    //public HeistController HeistController { get; private set; }

    public float StepDelayTime = 3f;

    public BaseEvent BaseEvent { get; private set; }
    public HeistLog Log { get; private set; }

    /// <summary>
    /// Instanciates an event type dynamically based on argument.
    /// </summary>
    /// <param name="eventType">Heist Event Type to use here</param>
    public void AssociateEvent(HEventType.HType eventType)
    {
        this.eventType = eventType;
        // BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        var type = HEventType.GetEventComponentType(eventType);
        BaseEvent = Activator.CreateInstance(type) as BaseEvent;
        eventState = HEventState.IdleUnfinished;
    }

    /// <summary>
    /// Transforms the current event to a different (or I guess same) event type, resets progress and restarts
    /// </summary>
    /// <param name="eventType">The event type to mutate into</param>
    public void MutateEvent(HEventType.HType eventType)
    {
        // Destroy(BaseEvent);
        AssociateEvent(eventType);
        // BaseEvent = this.gameObject.AddComponent(HEventType.GetEventComponentType(eventType)) as BaseEvent;
        BaseEvent.EnemyCrew = possesedEnemies;
        BeginHeistEvent();
    }

    /// <summary>
    /// Used to update the state of the event (unfinished, running, finished, ...) and notifying the storyteller
    /// Notify's the storyteller even if the state is "changed" to the same state
    /// (this can be used as a sort of game loop)
    /// </summary>
    /// <param name="state">Heist Event State to change to</param>
    public void ChangeHeistEventState(HEventState state)
    {
        // TODO: I may want this to be handled by the HeistController
        eventState = state;
        Storyteller.Instance.heistEventStateChanged.Invoke(this);
    }

    public BaseEvent GetBaseEvent() { return BaseEvent; }
    public HEventType.HType GetEventType() { return eventType; }
    public HEventState GetEventState() { return eventState; }

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
    /// <param name="crew">CrewController to associate as player crew</param>
    public void CrewIntake(CrewController crew)
    {
        Debug.Log("Taking in Crew");
        possesedCrew = crew;
    }

    /// <summary>
    /// Hands off the crew to the next node
    /// </summary>
    /// <param name="nextNode">EventController of the node to pass the crew off to</param>
    /// <returns>The provided argument, nextNode</returns>
    public EventController CrewPassToNext(EventController nextNode)
    {
        Debug.Log("Crew Moved To Next Node");
        Log.GetCurrent().ShortDescription = "The crew moves deeper into the plant..";
        //Log.GetCurrent().Duration = node.GetLineLength() * 10f;
        Log.GetCurrent().Duration = 30f;
        //EventController nextNode = node.GetDownstreamNode().eventController;
        nextNode.CrewIntake(possesedCrew);
        possesedCrew = null;
        // nextNode.enabled = true;
        // this.enabled = false;
        return nextNode;
    }


    private void BeginHeistEvent()
    {
        ChangeHeistEventState(HEventState.Begin);
        Debug.Log("-----------------------");
        Debug.Log("Begin Heist Event");
        BaseEvent.EventStart(possesedCrew, Log);
        BaseEvent.MyNameIs();
        Debug.Log($"Event Progress: {BaseEvent.GetProgress()}%");
    }
    /// <summary>
    /// Primarily performs the events (BaseEvent) EventStart method
    /// Accepts in the HeistLog to allow the current entry to be modified
    /// </summary>
    /// <param name="log">The heist event log</param>
    public void BeginHeistEvent(HeistLog log)
    {
        Log = log;
        //HeistController = heistController;
        BeginHeistEvent();
    }

    /// <summary>
    /// Main game loop, this steps the event and relies on the invariant that the event will
    /// eventually return true for HasSucceeded or HasFailed.
    /// Once success or failure, changes state
    /// </summary>
    public void HeistEventLoop()
    {
        ChangeHeistEventState(HEventState.Running);
        BaseEvent.StepEvent();
        if (BaseEvent.HasSucceeded() || BaseEvent.HasFailed()) ChangeHeistEventState(HEventState.Ending);
    }

    // this is under reconstruction as the functionality needs to be moved to the heist controller
    /// <summary>
    /// Cleans up the event and calls the BaseEvent type EventEnd method
    /// And then changes state to whether the event succeeded or failed
    /// </summary>
    public void EndHeistEvent()
    {
        HeistLogEntry entry = Log.GetCurrent();

        Debug.Log("End Heist Event");
        BaseEvent.EventEnd();
        // The rest of this could possibly be put inside the base event
        if (BaseEvent.HasFailed())
        {
            // TODO: FIX event node with no enemies, ?get lucky? phew
            if (eventType != HEventType.HType.Cmbt_Combat)
            {
                //Debug.Log("Enemy Spotted!!");
                //entry.ShortDescription = "You've been spotted! Get ready for combat!";
                ChangeHeistEventState(HEventState.DoneFailure);
                //entry.EntryColor = Color.red;
                MutateEvent(HEventType.HType.Cmbt_Combat);
                return;
            }
            //Debug.Log("Heist FAILED!!");
            //entry.ShortDescription = "Your crew is dead or captured. Heist Failed!";
            ChangeHeistEventState(HEventState.DoneFailure);
            //entry.EntryColor = Color.red;
            return;
        }
        /*if (node.GetDownstreamNode() == null)
        {
            Debug.Log("Finished Heist");
            entry.EntryColor = Color.green;
            entry.ShortDescription = "Your crew has successfully retrieved the package";
            ChangeHeistEventState(HEventState.DoneSuccess);
            return;
        }*/
        ChangeHeistEventState(HEventState.DoneSuccess);
        //entry.EntryColor = Color.green;
        //entry.ShortDescription = "Your crew has succeeded.";
        return;
    }
}
