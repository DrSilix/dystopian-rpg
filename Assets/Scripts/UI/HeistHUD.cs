using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeistHUD : IMenu
{
    private Label logDisplay;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        logDisplay = rootElem.Q("game-log") as Label;
    }

    public void RegisterCallbacks()
    {
        GameLog.Instance.LogUpdated += UpdateLogDisplay;
    }

    public void UpdateLogDisplay(object sender, EventArgs e)
    {
        logDisplay.text = ((GameLog)sender).GetGameLog();
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "attributes":
                break;
            case "return":
                break;
        }
    }

    public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    private void CallLoadMenu(string menuName, bool isChild, object passInfo)
    {
        LoadMenu.Invoke(this, (menuName, isChild, passInfo));
    }

    public event EventHandler<object> UnloadMenu;

    private void CallUnloadMenu(object passInfo)
    {
        UnloadMenu.Invoke(this, passInfo);
    }

    public void UnregisterCallbacks()
    {
        GameLog.Instance.LogUpdated -= UpdateLogDisplay;
    }

    public void Update() { }
}
