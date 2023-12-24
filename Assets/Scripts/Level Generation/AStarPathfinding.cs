/* initialize a grid board object
 * assign random weights to the grid
 * create a way to compare two grid states
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils;

public class AStarPathfinding
{
    int numNodes;
    int width, height;
    public AStarPathfinding(int numNodes, int width, int height, Vector2Int start, Vector2Int end)
    {
        this.numNodes = numNodes;
        this.width = width;
        this.height = height;

        int[,] startingState = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                startingState[i,j] = Random.Range(1, 256);
            }
        }

        Vector2Int[] nodes = new Vector2Int[numNodes];
        nodes[0] = start;
        if (end != null) { nodes[nodes.Length - 1] = end; }

        AStarNodeGrid initialState = new AStarNodeGrid(width, height, nodes);

        PriorityQueue<AStarNodeGrid, int> queue = new PriorityQueue<AStarNodeGrid, int>();



        // generate a grid with random weights
    }

    /*public class SearchNode : IComparable<SearchNode>
    {

    }*/
}
