using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNodeController : MonoBehaviour
{
    [SerializeField]
    private EventNodeController downstreamConnectedNode, upstreamConnectedNode;
    [SerializeField]
    private GameObject line;
    [SerializeField]
    private float lineLength;

    public Color lineColor = Color.white;
    public float lineWidth = .01f;
    public Material lineMaterial;
    public SpriteRenderer spriteRenderer;
    public EventController eventController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public EventNodeController GetDownstreamConnectedNode() { return downstreamConnectedNode; }

    public EventNodeController GetUpstreamConnectedNode() { return upstreamConnectedNode; }

    public GameObject GetLinePathToNext() { return line; }

    public float GetLineLength() { return lineLength; }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void BuildConnectingLine(Vector3 endPoint)
    {
        Vector3 startPoint = this.transform.position;
        line = new GameObject();
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, startPoint + Vector3.forward);
        lr.SetPosition(1, endPoint + Vector3.forward);
        lineLength = Vector3.Distance(endPoint, startPoint);
        line.transform.parent = this.transform;
    }

    // Upstream is back towards the start
    public void ConnectUpstreamNode(EventNodeController node)
    {
        upstreamConnectedNode = node;
    }

    // Downstream is from the start to the end
    public void ConnectDownstreamNode(EventNodeController node)
    {
        downstreamConnectedNode = node;
    }
}
