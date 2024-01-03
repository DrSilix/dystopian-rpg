using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public List<uiAsset> uiAssets;

    private Dictionary<string, VisualTreeAsset> _uiAssets;

    // Start is called before the first frame update
    void Start()
    {
        foreach (uiAsset asset in uiAssets)
        {
            _uiAssets.Add(asset.name, asset.asset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class uiAsset
    {
        public string name;
        public VisualTreeAsset asset;
    }
}
