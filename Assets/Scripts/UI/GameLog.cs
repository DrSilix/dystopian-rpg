using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLog : MonoBehaviour
{        
    public int linesToKeep = 4;
    
    private UIDocument doc;
    private VisualElement root;
    private Label log;

    private Queue<string> logList;

    public static GameLog Instance { get; private set; }

    private void Awake()
    {
        doc = this.GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        log = root.Q("game-log") as Label;
        logList = new Queue<string>();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void PostMessageToLog(string message)
    {
        logList.Enqueue(message);
        if(logList.Count > linesToKeep ) {
            logList.Dequeue();
            Debug.Log("DEQUEUED MESSAGE");
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string line in logList)
        {
            stringBuilder.Insert(0, line + "\n");
        }
        log.text = stringBuilder.ToString();
    }
}
