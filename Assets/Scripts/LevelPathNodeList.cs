using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LevelPathNodeList : MonoBehaviour
{
    public LevelPathNodeDetailsSO DetailsSO;
    public List<GameObject> LevelPathNodes;
    public List<GameObject> ReturnLevelPathNodes;
}
