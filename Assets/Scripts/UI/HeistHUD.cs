using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeistHUD : IMenu
{
    private Label logDisplay;

    private GameObject skyscraperBG;
    private GameObject travellingBG;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        logDisplay = rootElem.Q("game-log") as Label;

        skyscraperBG = Camera.main.transform.GetChild(0).GetChild(0).gameObject;
        travellingBG = Camera.main.transform.GetChild(0).GetChild(1).gameObject;
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

    public void SendMenuNewInfo(object info) {
        if (info.GetType() != typeof(EventController)) return;
        EventController heistEvent = info as EventController;
        switch (heistEvent.GetEventType())
        {
            case HEventType.HType.Pre_Navigating:
                switch (heistEvent.GetEventState())
                {
                    case HEventState.Begin:
                        travellingBG.SetActive(true);
                        break;
                    case HEventState.DoneFailure:
                    case HEventState.DoneSuccess:
                        skyscraperBG.SetActive(true);
                        travellingBG.SetActive(false);
                        break;
                }
                break;
            case HEventType.HType.Pst_ReturnHome:
                switch (heistEvent.GetEventState())
                {
                    case HEventState.Begin:
                        travellingBG.SetActive(true);
                        skyscraperBG.SetActive(false);
                        break;
                    case HEventState.DoneFailure:
                    case HEventState.DoneSuccess:
                        CallLoadMenu("SafeHouseMenu", false, heistEvent.GetEventState());
                        break;
                }
                break;
        }
    }
}
