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

    public void CrewIntake(CrewController Crew)
    {
        Debug.Log("Taking in Crew");
        possesedCrew = Crew;
        Crew.transform.position = this.gameObject.transform.position;
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
            return;
        }
        node.SetColor(Color.grey);
        TransportCrewToNextNode();
    }
}
