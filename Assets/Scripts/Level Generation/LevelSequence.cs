using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSequence
{
    private float doubleProbability = 0.6f;
    private Cluster first, last;
    private int size;
    public class Cluster
    {
        public int[] nodes = new int[2];
        public Cluster next, prev;
        public bool pullable;
        public int numTrailingPullable;
        public Cluster pullParent;
        public bool isSingle;

        public override string ToString()
        {
            if (nodes[1] != -1) return nodes[0].ToString() + nodes[1].ToString();
            return nodes[0].ToString();
        }
    }

    public LevelSequence(int numNodes, int numBacktracks)
    {
        first = null; last = null;
        size = 0;
        for (int i = 0; i < numNodes; i++)
        {
            bool isDouble = Random.Range(0.0f, 1.0f) < doubleProbability;
            if (i == 0 || i >= numNodes - 2) isDouble = false;
            if (isDouble)
            {
                AddNext(i, i + 1, false);
                doubleProbability = 0.5f;
                i++;
                continue;
            }
            AddNext(i, -1, i == numNodes - 1);
            doubleProbability = 0.8f;
        }
        AddBacktracks(numBacktracks, 2);
    }

    private void AddBacktracks(int numBacktracks, int maxNumClustersToPullWith)
    {
        Cluster current = first;
        List<Cluster> pullableClusters = new List<Cluster>();
        int clustersToPull = 0;
        while (current != null)
        {
            if (current.pullable)
            {
                pullableClusters.Add(current);
            }
            current = current.next;
        }
        if (pullableClusters.Count == 0) return;
        clustersToPull = pullableClusters.Count;
        if (clustersToPull > numBacktracks) clustersToPull = numBacktracks;

        for (int i = 0; i < clustersToPull; i++)
        {
            int clustersToPullWith = 0;
            if (maxNumClustersToPullWith > 0) { clustersToPullWith = GenerateRandomNumPulls(maxNumClustersToPullWith); }
            Cluster currPull = pullableClusters[i];
            if (clustersToPullWith > currPull.numTrailingPullable) clustersToPullWith = currPull.numTrailingPullable;
            PullCluster(currPull, clustersToPullWith);
        }
    }

    private void PullCluster(Cluster focus, int numToPullWith)
    {
        Cluster b = focus.prev;  // a b c ... e; c = focus; e = last pull with cluster
        Cluster a = b.prev;
        Cluster c = focus;
        Cluster e = focus;
        if (numToPullWith > 0)
        {
            e = c;
            for (int i = 0; i < numToPullWith; i++)
            {
                e = e.next;
            }
        }
        Cluster f = e.next;
        a.next = c;
        b.prev = e;
        b.next = f;
        f.prev = b;
        c.prev = a;
        e.next = b;
    }

    private int GenerateRandomNumPulls(int max)
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

    private void AddNext(int a, int b, bool isSequenceLast)
    {
        Cluster oldLast = last;
        last = new Cluster();
        last.nodes[0] = a;
        last.nodes[1] = b;
        last.next = null;
        last.prev = null;
        last.pullable = false;
        last.numTrailingPullable = 0;
        last.isSingle = b == -1;

        if (isEmpty()) { first = last; }
        else
        {
            last.prev = oldLast;
            if (oldLast != null)
            {
                oldLast.next = last;

                if (last.nodes[1] != -1 && oldLast.nodes[1] != -1) last.pullable = true;
                if (last.nodes[1] == -1 && !isSequenceLast)
                {
                    if (oldLast.pullable) last.pullParent = last.prev;
                    if (oldLast.pullParent != null) last.pullParent = oldLast.pullParent;
                    if (last.pullParent != null) last.pullParent.numTrailingPullable++; // TODO: remove last node from trailingpullable
                }
            }
        }
        size++;
    }

    public Cluster getFirstCluster() { return first; }
    public Cluster getLastCluster() { return last; }

    public bool isEmpty() { return size == 0; }
    public int getSize() { return size; }
    public override string ToString()
    {
        Cluster current = first;
        string result = "";
        int failSafeNum = 100, f = 0;
        if (current == null) return "";
        while (current != null)
        {
            if (++f >= failSafeNum) { break; }
            result += current.ToString();
            if (current.next != null) { result += "-"; }
            current = current.next;
        }
        return result;
    }

}
