using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Sits at the top level of the individual Heist (quest/mission) and interfaces with the event controllers to drive the heist forward
/// </summary>
public class HeistController : MonoBehaviour
{
    public float stepDelay = 2f;
    public int testCounterStop = 0;
    [field: SerializeField] public GameObject WorldControllerPrefab { get; set; }
    [field: SerializeField] public LevelPathNodeList LevelOnePathNodeList { get; set; }
    public Material playerMaterial;
    public Material zoneMaterial;

    public HeistLog Log { get; set; }

    private WorldController worldController;
    private LevelPathNodeDetailsSO levelPathNodeDetailsSO;
    private List<EventController> events;
    private List<GameObject> nodes;
    private Storyteller storyteller;
    private DateTime heistStartTime;
    private DateTime lastLogEntryTime;
    private HeistStepCounter stepCounter;
    private int currentEventPointer;

    /// <summary>
    /// Initializes the heist
    /// can be called to reset an already progressed/completed heist
    /// </summary>
    public void Initialize()
    {
        storyteller = Storyteller.Instance;
        levelPathNodeDetailsSO = LevelOnePathNodeList.DetailsSO;
        stepCounter = new HeistStepCounter();
        events = new List<EventController>();
        nodes = new List<GameObject>();
        Log = new HeistLog();
        storyteller.Crew.ResetToFull();
        currentEventPointer = 0;
    }

    /// <summary>
    /// In a debug state ATM. This sets the unity random engine seed and simply copies the unity editor configured list of nodes and their metadata
    /// Building the eventcontrollers
    /// </summary>
    /// <param name="seed">The seed to initialize the unity random engine with</param>
    public void GenerateHeist(int seed)
    {
        Random.InitState(seed);

        // the path to object and return are just concatenated for now
        foreach (LevelPathNodeDetailsSO.PathNodeDetails detail in levelPathNodeDetailsSO.LevelPathNodeDetails)
        {
            EventController temp = new();
            temp.AssociateEvent(detail.EventType);
            temp.EnemyIntake(storyteller.GenerateEnemies(3));
            events.Add(temp);
        }

        foreach (LevelPathNodeDetailsSO.PathNodeDetails detail in levelPathNodeDetailsSO.ReturnLevelPathNodeDetails)
        {
            EventController temp = new();
            temp.AssociateEvent(detail.EventType);
            temp.EnemyIntake(storyteller.GenerateEnemies(2));
            events.Add(temp);
        }

        foreach (GameObject go in LevelOnePathNodeList.LevelPathNodes)
        {
            nodes.Add(go);
        }

        foreach (GameObject go in LevelOnePathNodeList.ReturnLevelPathNodes)
        {
            nodes.Add(go);
        }

        //CreateWorldController();
        // assign a list of event controllers to this
        //events = worldController.GenerateLevel();
    }

    //unused, moving away from a fully generated world
    private void CreateWorldController()
    {
        var temp = Instantiate(WorldControllerPrefab);
        worldController = temp.GetComponent<WorldController>();
    }

    /// <summary>
    /// Kicks off the heist game loop after setting up the initial state and first log entry
    /// </summary>
    public void StartHeist()
    {
        //events[0].enabled = true;
        events[0].CrewIntake(storyteller.Crew);
        Debug.Log(nodes[0].transform.position);
        storyteller.Crew.transform.position = nodes[0].transform.position;
        UpdateNodeDisplay(nodes[0], true);
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

    /// <summary>
    /// Main game loop
    /// This is invoked on a stepDelay second delay to produce a game loop
    /// Primarily sets up a new log entry and then calls the current eventcontroller methods based on the state machine
    /// </summary>
    private void StepHeist()
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
                UpdateNodeDisplay(nodes[currentEventPointer], false);
                currentEvent.CrewPassToNext(events[++currentEventPointer]);
                //storyteller.Crew.transform.position = nodes[currentEventPointer].transform.position;
                // HACK: this makes the current level transition between nodes smoothly. This may be final but probably not
                storyteller.Crew.GetComponent<MovePlayerCrew>().MoveTo(nodes[currentEventPointer].transform.position, stepDelay / 3);
                UpdateNodeDisplay(nodes[currentEventPointer], true);
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

    //In the current neon wireframe office layout version this highlights the current room/area and leaves
    //visited areas slighty greyed
    private void UpdateNodeDisplay(GameObject node, bool isActive)
    {
        MeshRenderer meshRenderer = node.GetComponent<MeshRenderer>();
        List<Material> newMaterials = new List<Material>();
        if (isActive)
        {
            newMaterials.Add(zoneMaterial);
            newMaterials.Add(playerMaterial);
        }
        else
        {
            newMaterials.Add(zoneMaterial);
            newMaterials.Add(zoneMaterial);
        }
        meshRenderer.SetMaterials(newMaterials);
    }

    //This is the important heist step counter. the count can be used to simulate to any point during a heist repeatedly, reproducably, effciently
    private int CountHeistStep()
    {

        Debug.Log($"Heist step: {++stepCounter.Count}");
        return stepCounter.Count;
    }

    // (T-O-D-O-: instead of ending the heist, we should go to a replay.) I think I'm leaning towards not presimulating, only ever simulating up to current
    /// <summary>
    /// Crafts the final log entry and stops the game loop
    /// </summary>
    /// <param name="isSuccess">is the heist successful</param>
    private void EndHeist(bool isSuccess)
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

    //unused
    public void DestroyWorldController()
    {
        Destroy(worldController.gameObject);
    }
}
