/*
 *      Purpose to build a heist "world" including directing level gen, crew building, event choices, difficulty...
 *      Is called on menu heist start
 *      Does not call any other classes
 *      
 *      When [heist start] clicked
 *      takes in:
 *      level params (doesn't take in number of nodes only events)
 *      a crew
 *      heist event progression params (how does mother want me to layout events)
 *      loot?
 *      objective? win/fail conditions?
 *           
 *      (some of these could have already been done with recon)
 *      TODO: determines what events go where, start, objective, end, enemy count, ...
 *          loot, objective and general placement (call game design gen class)
 *      TODO: generates a world and places events (call level gen class)
 *      TODO: initializes crew and places them at start (call crew initialization class)
 *      NO > initializes protagonist director (Storyteller)  .. storyteller persists, an individual "world"/heist does not
 *      NO > initializes antagonist director .. persists outside of "world"
 *      
 *      NOTE: should be generic / able to build a sub heist (from a failed event)
 *      
 *      Has a finished generating event that others can subscribe to
 */


using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles generation of the world space entities that make up the game world
/// This is generally the "game board"
/// </summary>
public class WorldController : MonoBehaviour
{
    public GameObject nodePrefab;
    public int numNodes;
    public int gridWidth = 16, gridHeight = 9;

    private List<EventController> eventControllers;

    // TODO: remove - replace functionality with a subscribable world gen finished method
    /// <summary>
    /// Starts the level by placing the crew in the first event node, and
    /// calling its BeginHeistEvent method
    /// </summary>
    public void StartLevel()
    {
        PlaceCrew();
        eventControllers[0].enabled = true;
        eventControllers[0].CrewIntake(Storyteller.Instance.Crew);
        eventControllers[0].BeginHeistEvent();
    }

    // TODO: remove - separate out into a separate class that handles only this type of stuff
    /// <summary>
    /// moves the crew game object to the first event (this has the camera as child which moves with it)
    /// </summary>
    void PlaceCrew()
    {
        GameObject crewGO = Storyteller.Instance.Crew.gameObject;
        crewGO.transform.position = eventControllers[0].gameObject.transform.position;
    }

    // TODO: remove - separate out to a level gen class
    /// <summary>
    /// Temporary first pass at generating a set of N nodes on a grid from point a to b using only right angles
    /// This also associates events and creates enemies to place in them at the moment
    /// </summary>
    public List<EventController> GenerateLevel()
    {
        eventControllers = new List<EventController>();
        GameObject previousNode = null;
        int[] prevGridPoints = new int[2];
        int gridX, gridY;
        NodeGrid grid = new(gridWidth, gridHeight, 0.25f);
        grid.DrawDebugGrid();
        bool alternate = false;
        for (int i = 0; i < numNodes; i++)
        {
            if (i == numNodes - 1) alternate = true;
            if (alternate)
            {
                int maxNumForward = (gridWidth - prevGridPoints[0]) - (numNodes - i);
                if (maxNumForward < 1) maxNumForward = 1;
                gridX = prevGridPoints[0] + Random.Range(1, maxNumForward);
                gridY = prevGridPoints[1];
            }
            else
            {
                gridX = prevGridPoints[0];
                gridY = Random.Range(0, gridHeight - 1);
                if (gridY == prevGridPoints[1]) gridY++;
            }
            if (i == 0) { alternate = true; }
            if (i == numNodes - 1) gridX = gridWidth - 1;

            Vector3 spawnPos = grid.getGridPointToWorldSpace(gridX, gridY);
            GameObject node = Instantiate(nodePrefab, spawnPos, Quaternion.identity, this.transform);
            AssignEvent(node, i, numNodes);
            eventControllers.Add(node.GetComponent<EventController>());
            node.name += i;
            if (i > 0)
            {
                ConnectNodes(previousNode, node);
            }
            previousNode = node;
            prevGridPoints[0] = gridX;
            prevGridPoints[1] = gridY;
            if (i > 0) alternate = !alternate;
        }

        return eventControllers;

        // This is from my backburner'ed idea of using an A* algorithm with decreasingly random
        // priorities to generate a more interesting level

        /*Vector2Int[] test = new Vector2Int[numNodes];
        test[0] = new Vector2Int(0, 5);
        test[1] = new Vector2Int(3, 5);
        AStarNodeGrid aStarNodeGrid = new AStarNodeGrid(gridWidth, gridHeight, test);
        int manhattan = aStarNodeGrid.manhattan();
        Debug.Log("Testing...");
        bool isComp = aStarNodeGrid.isCompletePath();
        Debug.Log("Testing...");
        int hash = aStarNodeGrid.GetHashCode();
        Debug.Log("Testing...");
        bool equals = aStarNodeGrid.Equals(aStarNodeGrid);
        Debug.Log("Testing...");
        List<AStarNodeGrid> moves = aStarNodeGrid.PotentialMoves();
        Debug.Log("Testing...");

        foreach (AStarNodeGrid g in moves)
        {
            Debug.Log(g.ToString());
        }*/
    }

    // TODO: mostly for debug, 
    private void AssignEvent(GameObject node, int nodeNumber, int numNodes)
    {
        HEventType.HType[] eTypes = new HEventType.HType[] {
            HEventType.HType.Pre_Navigating,
            HEventType.HType.Hst_Sneaking,
            HEventType.HType.Hst_Masquerading,
            HEventType.HType.Hst_Breaching,
            HEventType.HType.Hst_Hacking,
            HEventType.HType.Obj_StealData,
            HEventType.HType.Pst_ReturnHome};

        EventController eventController = node.GetComponent<EventController>();
        eventController.enabled = false;

        // first event is navigation
        if (nodeNumber == 0)
        {
            eventController.AssociateEvent(eTypes[0]);
            return;
        }

        // last event is return home
        if (nodeNumber == numNodes - 1)
        {
            eventController.AssociateEvent(eTypes[6]);
            return;
        }

        CrewController enemies;

        // third from last is the objective event
        if (nodeNumber == numNodes - 3)
        {
            eventController.AssociateEvent(eTypes[5]);
            enemies = Storyteller.Instance.GenerateEnemies(Random.Range(2, 5));
            enemies.gameObject.transform.parent = node.transform;
            eventController.EnemyIntake(enemies);
            return;
        }

        // everything in-between is a random other event (no longer random, cycles through list)
        //eventController.AssociateEvent(eTypes[Random.Range(1,5)]);
        eventController.AssociateEvent(eTypes[(nodeNumber % 4 + 1)]);
        enemies = Storyteller.Instance.GenerateEnemies(Random.Range(1, 3));
        enemies.gameObject.transform.parent = node.transform;
        eventController.EnemyIntake(enemies);
    }

    // TODO: remove - separate out to the level gen class
    // connects two nodes together building a bi-directional linked list
    void ConnectNodes(GameObject prev, GameObject curr)
    {
        Node currentNode = curr.GetComponent<Node>();
        Node previousNode = prev.GetComponent<Node>();
        currentNode.ConnectUpstreamNode(previousNode);
        previousNode.ConnectDownstreamNode(currentNode);
        previousNode.BuildConnectingLine(curr.transform.position);
    }
}
