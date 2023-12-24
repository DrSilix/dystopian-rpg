using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseEvent : MonoBehaviour
{

    [SerializeField]
    private int difficulty;
    [SerializeField]
    private int neededSuccesses;
    [SerializeField]
    private int maxFails;
    [SerializeField]
    private int successes;
    [SerializeField]
    private int failures;
    [SerializeField]
    private int progress;
    [SerializeField]
    private CrewController crew;

    public int Difficulty { get => difficulty; set => difficulty = value; }
    public int NeededSuccesses { get => neededSuccesses; set => neededSuccesses = value; }
    public int MaxFails { get => maxFails; set => maxFails = value; }
    public int Successes { get => successes; set => successes = value; }
    public int Failures { get => failures; set => failures = value; }
    public int Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;
            if (progress > 100) progress = 100;
        }
    }
    public CrewController Crew { get => crew; set => crew = value; }

    public void EventStart(CrewController crew)
    {
        progress = 0;
        successes = 0;
        failures = 0;
        difficulty = Random.Range(8, crew.GetLuck() + ((20 - crew.GetLuck()) / 2));
        neededSuccesses = Random.Range(2, 7);
        MaxFails = Random.Range(5, 10);
        this.crew = crew;
    }

    public CrewController GetCrew() { return crew; }

    public void SetProgress(int p){ progress = p; }

    public abstract void EventEnd();

    public abstract bool StepEvent();

    public abstract void MyNameIs();

    public int GetProgress() { return progress; }

    public virtual bool HasSucceeded() { return progress >= 100; }
    public abstract bool HasFailed();

    public void MyBaseNameIs()
    {
        Debug.Log("BaseEvent");
    }
}
