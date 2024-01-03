using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public PanelSettings panelSettings;
    public List<uiAsset> uiAssetsList;


    private Dictionary<string, VisualTreeAsset> uiAssets;
    private List<UIDocument> uiDocs;

    private List<IMenu> uiScripts;
    private int currentMenu;

    // Start is called before the first frame update
    void Start()
    {
        uiAssets = new Dictionary<string, VisualTreeAsset>();
        
        foreach (uiAsset asset in uiAssetsList)
        {
            uiAssets.Add(asset.name, asset.asset);
        }

        uiDocs = new List<UIDocument>();
        uiScripts = new List<IMenu>();

        for (int i = 0; i < 3; i++)
        {
            GameObject newGO = new GameObject();
            newGO.name = "UI Level " + (i + 1);
            newGO.transform.parent = transform;
            UIDocument newDoc = newGO.AddComponent<UIDocument>();
            newDoc.panelSettings = panelSettings;
            newDoc.sortingOrder = i + 10;
            uiDocs.Add(newDoc);
            uiScripts.Add(null);
        }
        currentMenu = 0;

        LoadMenu("CrewMenu", false, null);
    }

    public void LoadMenu(string menuName, bool isChild, object passInfo)
    {
        if (!isChild && uiScripts[currentMenu] != null) {
            uiScripts[currentMenu].UnregisterCallbacks();
            uiScripts[currentMenu].LoadMenu -= LoadMenu;
            uiScripts[currentMenu].UnloadMenu -= UnloadMenu;
        }
        if (isChild) currentMenu++;
        uiDocs[currentMenu].visualTreeAsset = uiAssets[menuName];
        Type script = Type.GetType(menuName);
        IMenu menu = Activator.CreateInstance(script) as IMenu;
        menu.InitializeMenu(uiDocs[currentMenu], passInfo);
        menu.RegisterCallbacks();
        menu.LoadMenu += LoadMenu;
        menu.UnloadMenu += UnloadMenu;
        uiScripts[currentMenu] = menu;
    }

    private void LoadMenu(object sender, (string menuName, bool isChild, object passInfo) e)
    {
        LoadMenu(e.menuName, e.isChild, e.passInfo);
    }

    // can we assume the current menu is the one being unloaded?
    // should be the user is clicking the "return" button on the current menu
    public void UnloadMenu(object passInfo)
    {
        uiScripts[currentMenu].UnregisterCallbacks();
        uiScripts[currentMenu].LoadMenu -= LoadMenu;
        uiScripts[currentMenu].UnloadMenu -= UnloadMenu;
        uiDocs[currentMenu].visualTreeAsset = null;
        currentMenu--;
        uiScripts[currentMenu].InitializeMenu(uiDocs[currentMenu], passInfo);
    }

    private void UnloadMenu(object sender, object e)
    {
        UnloadMenu(e);
    }

    [Serializable]
    public class uiAsset
    {
        public string name;
        public VisualTreeAsset asset;
    }
}
