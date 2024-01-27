using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public SaveGameManager Instance {  get; private set; }
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
