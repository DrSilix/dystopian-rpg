using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLog
{        
    public int linesToKeep = 8;

    private Queue<string> logList = new Queue<string>();

    public static GameLog Instance { get; private set; }

    public GameLog()
    {        
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
    }

    public event EventHandler LogUpdated;

    public void PostMessageToLog(string message)
    {
        logList.Enqueue(message);
        if(logList.Count > linesToKeep ) {
            logList.Dequeue();
            //Debug.Log("DEQUEUED MESSAGE");
        }
        LogUpdated.Invoke(this, null);
    }

    public string GetGameLog()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string line in logList)
        {
            stringBuilder.Insert(0, line + "\n");
        }
        return stringBuilder.ToString();
    }
}
