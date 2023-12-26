/*
 * Handles the general non-game related properties and functions of the nodes
 * contains references to upstream (back towards start) and downstream (towards finish) nodes
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Node downstreamConnectedNode, upstreamConnectedNode;
    [SerializeField]
    private GameObject line;
    [SerializeField]
    private float lineLength;

    public Color lineColor = Color.white;
    public float lineWidth = .01f;
    public Material lineMaterial;
    public SpriteRenderer spriteRenderer;
    public EventController eventController;

    public Node GetDownstreamNode() { return downstreamConnectedNode; }

    public Node GetUpstreamNode() { return upstreamConnectedNode; }

    public GameObject GetLine() { return line; }

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
    public void ConnectUpstreamNode(Node node)
    {
        upstreamConnectedNode = node;
    }

    // Downstream is from the start to the end
    public void ConnectDownstreamNode(Node node)
    {
        downstreamConnectedNode = node;
    }
}
