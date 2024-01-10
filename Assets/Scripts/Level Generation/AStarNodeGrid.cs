using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStarNodeGrid
{
    int width, height;
    Vector2Int[] nodes;
    int currentNode;
    bool endSpecified;
    bool[,] occupied;

    public AStarNodeGrid(int width, int height, Vector2Int[] nodes)
    {
        this.width = width;
        this.height = height;
        endSpecified = nodes[nodes.Length - 1] != null;
        this.nodes = new Vector2Int[nodes.Length];

        occupied = new bool[width, height];
        for (int i = 0; i < nodes.Length && nodes[i] != null; i++) {
            if (!DebugTools.InBounds(i, nodes.Length)) throw new Exception("AStarNodeGrid-Main-i-bounds");
            if (nodes[i] == null) throw new Exception("AStarNodeGrid-Main-i");
            this.nodes[i] = nodes[i];
            currentNode = i;
            occupied[nodes[i].x, nodes[i].y] = true;
            if (i == 0) continue;
            bool isXAligned = nodes[i - 1].y == nodes[i].y;
            if (isXAligned )
            {
                for (int j = nodes[i - 1].x; ;)
                {
                    if (j < nodes[i].x) j++;
                    if (j > nodes[i].x) j--;
                    if (j == nodes[i].x) break;
                    if (!DebugTools.InBounds(j, occupied.GetLength(0))) throw new Exception("AStarNodeGrid-Main-xj-bounds");
                    occupied[j, nodes[i].y] = true;
                }
            }
            else
            {
                for (int j = nodes[i - 1].y; ;)
                {
                    if (j < nodes[i].y) j++;
                    if (j > nodes[i].y) j--;
                    if (j == nodes[i].y) break;
                    if (!DebugTools.InBounds(j, occupied.GetLength(1))) throw new Exception("AStarNodeGrid-Main-xj-bounds");
                    occupied[nodes[i].x, j] = true;
                }
            }
        }
    }

    public override string ToString()
    {
        string result = string.Empty;
        int i = 0;
        while (i < nodes.Length && nodes[i] != null)
        {
            result += nodes[i].ToString();
            i++;
        }
        return result;
    }

    public List<AStarNodeGrid> PotentialMoves()
    {
        List<AStarNodeGrid> grids = new List<AStarNodeGrid>();
        grids = grids.Concat(GetDirectionsMoves(Vector2Int.up)).ToList();
        grids = grids.Concat(GetDirectionsMoves(Vector2Int.down)).ToList();
        grids = grids.Concat(GetDirectionsMoves(Vector2Int.left)).ToList();
        grids = grids.Concat(GetDirectionsMoves(Vector2Int.right)).ToList();
        return grids;
    }

    private List<AStarNodeGrid> GetDirectionsMoves(Vector2Int dir)
    {
        Vector2Int oldVal = nodes[currentNode + 1];
        List<AStarNodeGrid> result = new List<AStarNodeGrid>();
        Vector2Int moved = nodes[currentNode] + dir;
        int max = 0;
        if (dir == Vector2Int.down) max = Vector2IntTools.manhattanTo(nodes[currentNode], new Vector2Int(nodes[currentNode].x, 0));
        if (dir == Vector2Int.up) max = Vector2IntTools.manhattanTo(nodes[currentNode], new Vector2Int(nodes[currentNode].x, height - 1));
        if (dir == Vector2Int.left) max = Vector2IntTools.manhattanTo(nodes[currentNode], new Vector2Int(0, nodes[currentNode].y));
        if (dir == Vector2Int.right) max = Vector2IntTools.manhattanTo(nodes[currentNode], new Vector2Int(width - 1, nodes[currentNode].y));
        if (max > 6) max = 6;

        if (occupied[moved.x, moved.y]) return result;
        for (int i = 0; i < max && !occupied[moved.x, moved.y]; i++)
        {
            if (!DebugTools.InBounds(currentNode + 1, nodes.Length)) throw new Exception("GetDirectionsMoves");
            nodes[currentNode + 1] = moved;
            result.Add(new AStarNodeGrid(width, height, nodes));
            moved += dir;
            // if (!DebugTools.inBounds(moved.x, occupied.GetLength(0)) || !DebugTools.inBounds(moved.y, occupied.GetLength(1))) throw new Exception("GetDirectionsMoves");
        }
        nodes[currentNode + 1] = oldVal;
        return result;
    }

    public int manhattan()
    {
        if (endSpecified) return Vector2IntTools.manhattanTo(nodes[currentNode], nodes[nodes.Length - 1]);
        else return width - nodes[currentNode].x;
    }

    public bool isCompletePath() { return currentNode == nodes.Length - 1; }

    public override bool Equals(object other)
    {
        if (!(other is AStarNodeGrid))
        {
            return false;
        }
        return Enumerable.SequenceEqual(nodes, ((AStarNodeGrid)other).nodes);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        for (int i = 0; i < nodes.Length && nodes[i] != null; i++)
        {
            if (!DebugTools.InBounds(i, nodes.Length)) throw new Exception("GetHashCode");
            hash = unchecked((hash * 31) ^ nodes[i].GetHashCode());
        }
        return hash;
    }
}
