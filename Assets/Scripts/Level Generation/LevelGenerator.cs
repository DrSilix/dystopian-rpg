using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator
{
    int width, height;
    int n;
    int numBacktracks;
    int numCrosses;
    Vector2Int[] level;
    LevelSequence sequence;
    public LevelGenerator (int width, int height, int numNodes, int numBacktracks, int numCrosses)
    {
        this.width = width;
        this.height = height;
        n = numNodes;
        this.numBacktracks = numBacktracks;
        this.numCrosses = numCrosses;
        level = new Vector2Int[n];
        sequence = new LevelSequence(n, numBacktracks);

        level = BuildLevel();
    }


    // sequence comes in the form (except with ints)
    // A DE BC FG H IJ K (1 45 23 67 8 910 11)
    // this represents a 1D x-axis of nodes where doubles are two nodes vertical
    private Vector2Int[] BuildLevel()
    {
        Vector2Int[] result = new Vector2Int[n];
        result[0] = new Vector2Int(0, Random.Range(0, height - 2) + 1);  // starting node
        LevelSequence.Cluster first = sequence.getFirstCluster();
        LevelSequence.Cluster current = first.next;
        int lastNodeIndex = 0;
        for (int cl = 1; current.next != null; cl++)
        {
            int clusterPreferedXLocation = getClusterPreferedXLocation(cl);
            int actualXLocation = clusterPreferedXLocation + ((Random.Range(0, 2) * 2 - 1) * GenerateRandomXVariance(3));
            if (actualXLocation <= result[lastNodeIndex].x) actualXLocation = result[lastNodeIndex].x + 1;
            if (current.isSingle)
            {
                result[current.nodes[0]] = new Vector2Int(actualXLocation, result[lastNodeIndex].y);
                lastNodeIndex = current.nodes[0];
            }
            else
            {
                result[current.nodes[0]] = new Vector2Int(actualXLocation, result[lastNodeIndex].y);
                result[current.nodes[1]] = new Vector2Int(actualXLocation, getNodeYLocation(current, result));
                lastNodeIndex = current.nodes[1];
            }
            current = current.next;
        }
        Debug.Assert(current != null || current.next == null, current);
        result[result.Length-1] = new Vector2Int(width - 1, result[result.Length-2].y);  // starting node

        return result;
    }

    private int getNodeYLocation(LevelSequence.Cluster cluster, Vector2Int[] nodePositions)
    {
        // bool dependentOnPast = false;
        return Random.Range(0, height);
    }

    private int GenerateRandomXVariance(int max)
    {
        int rand = Random.Range(1, 101);
        float temp = rand;
        for (int i = 0; i < max + 1; i++)
        {
            temp -= (1 / Mathf.Pow(2, i + 1)) * 100;
            if (temp < 0) return i;
        }
        return 0;
    }

    private int getClusterPreferedXLocation (int index)
    {
        int sequenceWorkingWidth = sequence.getSize() - 1;
        return Mathf.RoundToInt(((float)index / sequenceWorkingWidth) * width);
    }

    public Vector2Int getNodeXYbyIndex(int index)
    {
        return level[index];
    }

    public LevelSequence getLevelSequence()
    {
        return sequence;
    }
}
