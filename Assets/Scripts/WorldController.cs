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


using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    public GameObject nodePrefab;
    public int numNodes;
    public int gridWidth = 16, gridHeight = 9;

    private EventController InitialEvent;

    // TODO: remove - replace functionality with a subscribable world gen finished method
    public void StartLevel()
    {
        PlaceCrew();
        InitialEvent.enabled = true;
        InitialEvent.CrewIntake(Storyteller.Instance.Crew);
        InitialEvent.BeginHeistEvent();
    }

    // TODO: remove - separate out into a separate class that handles only this type of stuff
    void PlaceCrew()
    {
        GameObject crewGO = Storyteller.Instance.Crew.gameObject;
        crewGO.transform.position = InitialEvent.gameObject.transform.position;
    }

    // TODO: remove - separate out to a level gen class
    public void GenerateLevel()
    {
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
            GameObject node = Instantiate(nodePrefab, spawnPos, Quaternion.identity);
            AssignEvent(node, i, numNodes);
            if (i == 0) { InitialEvent = node.GetComponent<EventController>(); }
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

        if (nodeNumber == 0)
        {
            eventController.AssociateEvent(eTypes[0]);
            return;
        }

        if (nodeNumber == numNodes - 1)
        {
            eventController.AssociateEvent(eTypes[6]);
            return;
        }

        CrewController enemies;

        if (nodeNumber == numNodes - 3)
        {
            eventController.AssociateEvent(eTypes[5]);
            enemies = Storyteller.Instance.GenerateEnemies(Random.Range(2, 4));
            enemies.gameObject.transform.parent = node.transform;
            eventController.EnemyIntake(enemies);
            return;
        }

        //eventController.AssociateEvent(eTypes[Random.Range(1,5)]);
        eventController.AssociateEvent(eTypes[(nodeNumber % 4 + 1)]);
        enemies = Storyteller.Instance.GenerateEnemies(Random.Range(1, 3));
        enemies.gameObject.transform.parent = node.transform;
        eventController.EnemyIntake(enemies);
    }

    // TODO: remove - separate out to the level gen class
    void ConnectNodes(GameObject prev, GameObject curr)
    {
        Node currentNode = curr.GetComponent<Node>();
        Node previousNode = prev.GetComponent<Node>();
        currentNode.ConnectUpstreamNode(previousNode);
        previousNode.ConnectDownstreamNode(currentNode);
        previousNode.BuildConnectingLine(curr.transform.position);
    }
}
