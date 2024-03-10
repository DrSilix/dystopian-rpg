using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelPathNodeList : MonoBehaviour
{
    public LevelPathNodeDetailsSO DetailsSO;
    public List<GameObject> LevelPathNodes;
    public List<GameObject> ReturnLevelPathNodes;
}
