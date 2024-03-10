using UnityEngine;

/// <summary>
/// Handles the general non-game related properties and functions of the nodes
/// contains references to upstream (back towards start) and downstream (towards finish) nodes
/// </summary>
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

    /// <summary>
    /// Gets the next node in the linked list
    /// </summary>
    /// <returns>the next Node in the series or null if none</returns>
    public Node GetDownstreamNode() { return downstreamConnectedNode; }
    /// <summary>
    /// Gets the previous node in the linked list
    /// </summary>
    /// <returns>the previous Node in the series or null if none</returns>
    public Node GetUpstreamNode() { return upstreamConnectedNode; }

    /// <summary>
    /// Gets the line that is drawn from this to the next node
    /// </summary>
    /// <returns>GameObject for the line</returns>
    public GameObject GetLine() { return line; }

    /// <summary>
    /// The length of the line that is created from this node to the next node
    /// </summary>
    /// <returns>the distance as a float</returns>
    public float GetLineLength() { return lineLength; }

    /// <summary>
    /// Sets the color of the nodes sprite
    /// </summary>
    /// <param name="color">The color to change the sprite to</param>
    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Builds a line from this node to the provided endPoint using LineRenderer
    /// and attaches the line as a child of this
    /// </summary>
    /// <param name="endPoint">The Vector3 to draw the line to</param>
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

    /// <summary>
    /// Associates the upstream node with this node (upstream is previous or towards the start)
    /// </summary>
    /// <param name="node">The node to associate as upstream</param>
    public void ConnectUpstreamNode(Node node)
    {
        upstreamConnectedNode = node;
    }

    /// <summary>
    /// Associates the downstream node with this node (downstream is next or towards the end)
    /// </summary>
    /// <param name="node">The node to associate as downstream</param>
    public void ConnectDownstreamNode(Node node)
    {
        downstreamConnectedNode = node;
    }
}
