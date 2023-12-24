using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealDataEvent : BaseEvent
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
        SetProgress(GetProgress() + 20);
		return true;
    }

    public override void MyNameIs()
    {
        Debug.Log("StealDataEvent");
    }

    public override bool HasFailed()
    {
        return false;
    }
}
