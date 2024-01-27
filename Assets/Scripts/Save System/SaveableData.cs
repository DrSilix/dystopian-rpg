using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableData
{
    public void RegisterThis()
    {

    }
    public abstract void SaveData(SaveData saveData);
    public abstract void LoadData(SaveData saveData);
}
