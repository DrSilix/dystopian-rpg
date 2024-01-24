using UnityEngine;
using System.Collections;

/***************************************************************************************
*    Author: Positive7 (discussions.unity username), xjcl (stackoverflow username)
*    Date: 01/23/2024
*    Availability: https://stackoverflow.com/questions/67704820/how-do-i-print-unitys-debug-log-to-the-screen-gui
*    
***************************************************************************************/
public class ZzzLog : MonoBehaviour
{
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    void Start()
    {
        Debug.Log("Started up logging.");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        GUIStyle temp = new GUIStyle();
        temp.fontSize = 26;
        temp.normal.textColor = Color.white;
        GUILayout.BeginArea(new Rect(100, (Screen.height * 0.15f + 50), Screen.width - 150, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()),temp);
        GUILayout.EndArea();
    }
}