using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public PanelSettings panelSettings;
    //public List<uiAsset> uiAssetsList;
    public VisualTreeAsset mainMenu;
    public AssetLabelReference assetLabelRef;

    private Dictionary<string, VisualTreeAsset> uiAssets;
    private List<UIDocument> uiDocs;

    private List<IMenu> uiScripts;
    private int currentMenu;
    private int currentMenuContainerSize;

    // Start is called before the first frame update
    void Start()
    {
        uiAssets = new Dictionary<string, VisualTreeAsset>();

        uiAssets.Add(mainMenu.name, mainMenu);

        LoadMenuList();

        uiDocs = new List<UIDocument>();
        uiScripts = new List<IMenu>();

        for (int i = 0; i < 3; i++)
        {
            BuildMenuGameObject(i);
        }
        currentMenu = 0;
        currentMenuContainerSize = 3;

        Storyteller.Instance.heistEventStateChanged += HeistEventStateChange;
        
        new GameLog();

        LoadMenu("MainMenu", false, null);
    }

    private async void LoadMenuList()
    {
        await Addressables.LoadAssetsAsync<object>(assetLabelRef, (a) =>
        {
            switch (a)
            {
                case VisualTreeAsset s:
                    uiAssets.Add(s.name, s);
                    break;
            }
        }).Task;
        Debug.Log("UI Loaded");
    }

    private void HeistEventStateChange(EventController e)
    {
        passCurrentMenuNewInfo(e);
    }

    private void passCurrentMenuNewInfo(object info)
    {
        uiScripts[currentMenu].SendMenuNewInfo(info);
    }

    private void BuildMenuGameObject(int id)
    {
        GameObject newGO = new GameObject();
        newGO.name = "UI Level " + (id + 1);
        newGO.transform.parent = transform;
        UIDocument newDoc = newGO.AddComponent<UIDocument>();
        newDoc.panelSettings = panelSettings;
        newDoc.sortingOrder = id + 10;
        uiDocs.Add(newDoc);
        uiScripts.Add(null);
    }

    public void LoadMenu(string menuName, bool isChild, object passInfo)
    {
        if (!isChild && uiScripts[currentMenu] != null) {
            uiScripts[currentMenu].UnregisterCallbacks();
            uiScripts[currentMenu].LoadMenu -= LoadMenu;
            uiScripts[currentMenu].UnloadMenu -= UnloadMenu;
        }
        if (isChild) currentMenu++;
        if (currentMenu >= currentMenuContainerSize) BuildMenuGameObject(currentMenu);
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
    public void UnloadMenu(object passInfo, bool resetParentMenu = true)
    {
        uiScripts[currentMenu].UnregisterCallbacks();
        uiScripts[currentMenu].LoadMenu -= LoadMenu;
        uiScripts[currentMenu].UnloadMenu -= UnloadMenu;
        uiScripts[currentMenu] = null;
        uiDocs[currentMenu].visualTreeAsset = null;
        currentMenu--;
        if (resetParentMenu)
        {
            uiScripts[currentMenu].UnregisterCallbacks();
            uiScripts[currentMenu].InitializeMenu(uiDocs[currentMenu], passInfo);
            uiScripts[currentMenu].RegisterCallbacks();
        }
        else
        {
            uiScripts[currentMenu].SendMenuNewInfo(passInfo);
        }
    }

    private void UnloadMenu(object sender, (object passInfo, bool resetParentMenu) e)
    {
        UnloadMenu(e.passInfo, e.resetParentMenu);
    }

    private void Update()
    {
        if (uiScripts == null || uiScripts[currentMenu] == null) return;
        uiScripts[currentMenu].Update();
    }

    [Serializable]
    public class uiAsset
    {
        public string name;
        public VisualTreeAsset asset;
    }
}
