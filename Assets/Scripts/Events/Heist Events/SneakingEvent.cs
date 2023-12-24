using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakingEvent : BaseEvent
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void EventEnd()
    {
        throw new System.NotImplementedException();
    }

    public override bool StepEvent()
    {
        int eventRoll = Random.Range(0, 20) + Difficulty;
        int crewRoll = Crew.GetLuckRoll();
        Debug.Log(eventRoll + " vs. " + crewRoll);
        if (crewRoll > eventRoll)
        {
            Successes++;
            Debug.Log("Success #" + Successes + " out of " + NeededSuccesses);
            Progress = Mathf.RoundToInt((float)(Successes * 100) / NeededSuccesses);
            return true;
        }
        Failures++;
        Debug.Log("FAILURE! #" + Failures + " out of " + MaxFails);
        return false;
    }

    public override void MyNameIs()
    {
        Debug.Log("SneakingEvent");
    }

    public override bool HasSucceeded()
    {
        return Successes >= NeededSuccesses;
    }

    public override bool HasFailed()
    {
        return Failures >= MaxFails;
    }
}
