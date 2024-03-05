using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeistController : MonoBehaviour
{
    public float stepDelay = 2f;
    public int testCounterStop = 0;
    [field: SerializeField] public GameObject WorldControllerPrefab { get; set; }
    [field: SerializeField] public LevelPathNodeList LevelOnePathNodeList { get; set; }

    public HeistLog Log { get; set; }

    private WorldController worldController;
    private LevelPathNodeDetailsSO levelPathNodeDetailsSO;
    private List<EventController> events;
    private Storyteller storyteller;
    private DateTime heistStartTime;
    private DateTime lastLogEntryTime;
    private HeistStepCounter stepCounter;
    private int currentEventPointer;

    public void Initialize()
    {
        storyteller = Storyteller.Instance;
        levelPathNodeDetailsSO = LevelOnePathNodeList.DetailsSO;
        stepCounter = new HeistStepCounter();
        events = new List<EventController>();
        Log = new HeistLog();
        storyteller.Crew.ResetToFull();
        currentEventPointer = 0;
    }
    public void GenerateHeist(int seed)
    {
        Random.InitState(seed);

        foreach (LevelPathNodeDetailsSO.PathNodeDetails detail in levelPathNodeDetailsSO.LevelPathNodeDetails)
        {
            EventController temp = new();
            temp.AssociateEvent(detail.EventType);
            temp.EnemyIntake(storyteller.GenerateEnemies(1));
            events.Add(temp);
        }

        foreach (LevelPathNodeDetailsSO.PathNodeDetails detail in levelPathNodeDetailsSO.ReturnLevelPathNodeDetails)
        {
            EventController temp = new();
            temp.AssociateEvent(detail.EventType);
            temp.EnemyIntake(storyteller.GenerateEnemies(1));
            events.Add(temp);
        }

        //CreateWorldController();
        // assign a list of event controllers to this
        //events = worldController.GenerateLevel();
    }

    private void CreateWorldController()
    {
        var temp = Instantiate(WorldControllerPrefab);
        worldController = temp.GetComponent<WorldController>();
    }

    public void StartHeist()
    {
        //events[0].enabled = true;
        events[0].CrewIntake(storyteller.Crew);
        heistStartTime = DateTime.Now;
        HeistLogEntry startingLogEntry = new HeistLogEntry();
        startingLogEntry.ParentEntry = null;
        startingLogEntry.EntryStartTime = heistStartTime;
        startingLogEntry.Duration = 0;
        startingLogEntry.ShortDescription = "Texico Water Plant - Retrieve the black box";
        startingLogEntry.Body = "The crew was hired by an unkown entity through their contact Jezzabelle Lightning. "
                                     + "A 3 man job they said. Well, the pay is good and how much trouble can a water plant be";
        startingLogEntry.EntryColor = Color.yellow;
        startingLogEntry.CurrentLocation = Storyteller.Instance.Crew.gameObject.transform.position;
        startingLogEntry.StepNumber = 0;
        Log.Add(startingLogEntry);
        StepHeist();
    }

    public void StepHeist()
    {
        HeistLogEntry currentLogEntry = new HeistLogEntry();
        Log.Add(currentLogEntry);
        currentLogEntry.EntryStartTime = Log.GetNextTime();
        currentLogEntry.Duration = 2f;
        currentLogEntry.MarkCrewLocation();
        CountHeistStep();
        currentLogEntry.StepNumber = stepCounter.Count;
        EventController currentEvent = events[currentEventPointer];
        HEventState currentState = currentEvent.GetEventState();
        switch (currentState)
        {
            case HEventState.IdleUnfinished:
                currentEvent.BeginHeistEvent(Log);
                break;
            case HEventState.Begin:
            case HEventState.Running:
                currentEvent.HeistEventLoop();
                break;
            case HEventState.Ending:
                currentEvent.EndHeistEvent();
                break;
            case HEventState.DoneSuccess:
                if (currentEventPointer >= events.Count - 1)
                {
                    // this hack ties back into the system for eventcontroller to pass a message to the UI
                    // HACK: HeistHUD should call main menu differently instead of this
                    currentEvent.ChangeHeistEventState(HEventState.HeistFinished);
                    EndHeist(true);
                    return;
                }
                // call local method to interpolate distance travelled to next node
                currentEvent.CrewPassToNext(events[++currentEventPointer]);
                break;
            case HEventState.DoneFailure:
                // HACK: same as above
                currentEvent.ChangeHeistEventState(HEventState.HeistFinished);
                EndHeist(false);
                return;
        }
        if (stepCounter.Count < testCounterStop) StepHeist();
        else Invoke(nameof(StepHeist), stepDelay);
    }

    public int CountHeistStep()
    {

        Debug.Log($"Heist step: {++stepCounter.Count}");
        return stepCounter.Count;
    }

    // TODO: instead of ending the heist, we should go to a replay.
    public void EndHeist(bool isSuccess)
    {
        HeistLogEntry finalLogEntry = new HeistLogEntry();
        finalLogEntry.ParentEntry = null;
        finalLogEntry.EntryStartTime = Log.GetNextTime();
        finalLogEntry.Duration = 0;
        finalLogEntry.ShortDescription = "Texico Water Plant - Retrieve the black box";
        finalLogEntry.Body = "The crew was successful. Time to get paid!";
        finalLogEntry.EntryColor = Color.green;
        finalLogEntry.CurrentLocation = Storyteller.Instance.Crew.gameObject.transform.position;
        finalLogEntry.StepNumber = stepCounter.Count + 1;
        Log.Add(finalLogEntry);
        CancelInvoke();
        //DestroyWorldController();
    }

    public void DestroyWorldController()
    {
        Destroy(worldController.gameObject);
    }
}
