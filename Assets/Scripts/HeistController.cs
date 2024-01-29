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

    private WorldController worldController;
    private List<EventController> events;
    private Storyteller storyteller;
    private HeistStepCounter stepCounter;
    private int currentEventPointer;
    private int testCounterStop = 480;

    public void Initialize()
    {
        storyteller = Storyteller.Instance;
        stepCounter = new HeistStepCounter();
        storyteller.Crew.ResetToFull();
        currentEventPointer = 0;
    }
    public void GenerateHeist(int seed)
    {
        Random.InitState(seed);
        CreateWorldController();
        events = worldController.GenerateLevel();
    }

    private void CreateWorldController()
    {
        var temp = Instantiate(WorldControllerPrefab);
        worldController = temp.GetComponent<WorldController>();
    }

    public void StartHeist()
    {
        events[0].enabled = true;
        events[0].CrewIntake(storyteller.Crew);
        StepHeist();
    }

    /*public void StepHeist()
    {
        CountHeistStep();
        HEventState state = events[currentEventPointer].StepEventController();
        bool heistIsFinished = AdvanceHeistOrFinish(state);
        if (heistIsFinished)
        {
            EndHeist();
            return;
        }
        if (stepCounter.Count < testCounterStop)
        {
            StepHeist();
            return;
        }
        Invoke("StepHeist", stepDelay);
    }**/

    public void StepHeist()
    {
        CountHeistStep();
        EventController currentEvent = events[currentEventPointer];
        HEventState currentState = currentEvent.GetEventState();
        switch (currentState)
        {
            case HEventState.IdleUnfinished:
                currentEvent.BeginHeistEvent(this);
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
                    // TODO: HeistHUD should call main menu differently instead of this
                    currentEvent.ChangeHeistEventState(HEventState.HeistFinished);
                    EndHeist(true);
                    return;
                }
                // call local method to interpolate distance travelled to next node
                currentEvent.CrewPassToNext();
                currentEventPointer++;
                break;
            case HEventState.DoneFailure:
                currentEvent.ChangeHeistEventState(HEventState.HeistFinished);
                EndHeist(false);
                return;
        }
        if (stepCounter.Count < testCounterStop) StepHeist();
        else Invoke(nameof(StepHeist), stepDelay);
    }

    public bool AdvanceHeistOrFinish(HEventState state)
    {
        switch (state)
        {
            case HEventState.DoneSuccess:
                if (currentEventPointer >= events.Count - 1) return true;
                currentEventPointer++;
                break;
            case HEventState.DoneFailure:
                return true;
        }
        return false;
    }

    public int CountHeistStep()
    {

        Debug.Log($"Heist step: {++stepCounter.Count}");
        return stepCounter.Count;
    }

    public void EndHeist(bool isSuccess)
    {
        CancelInvoke();
        DestroyWorldController();
    }

    public void DestroyWorldController()
    {
        Destroy(worldController.gameObject);
    }
}
