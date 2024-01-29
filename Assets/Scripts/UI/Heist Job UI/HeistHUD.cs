using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class HeistHUD : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private Label logDisplay;
    private Label statusDisplay;
    private Button button1;
    private Button button2;
    private Button button3;

    private GameObject skyscraperBG;
    private GameObject travellingBG;

    private GameObject DebugLogGO;
    private ZzzLog logComponent;

    private EventController heistEvent;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        logDisplay = rootElem.Q("game-log") as Label;
        statusDisplay = rootElem.Q("status") as Label;

        button1 = rootElem.Q("button1") as Button;
        button2 = rootElem.Q("button2") as Button;
        button3 = rootElem.Q("button3") as Button;

        DebugLogGO = GameObject.Find("DebugLog");
        logComponent = DebugLogGO.GetComponent<ZzzLog>();


        skyscraperBG = Camera.main.transform.GetChild(0).GetChild(0).gameObject;
        travellingBG = Camera.main.transform.GetChild(0).GetChild(1).gameObject;
    }

    public void RegisterCallbacks()
    {
        GameLog.Instance.LogUpdated += UpdateLogDisplay;
        button1.RegisterCallback<ClickEvent>(OnClick);
        button2.RegisterCallback<ClickEvent>(OnClick);
        button3.RegisterCallback<ClickEvent>(OnClick);
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
            case "button1":
                break;
            case "button2":
                float time = heistEvent.StepDelayTime;
                switch (time)
                {
                    case 3f:
                        time = 2f;
                        GameLog.Instance.PostMessageToLog($"Step delay changed to {time} seconds");
                        break;
                    case 2f:
                        time = 1f;
                        GameLog.Instance.PostMessageToLog($"Step delay changed to {time} seconds");
                        break;
                    case 1f:
                        time = 0.5f;
                        GameLog.Instance.PostMessageToLog($"Step delay changed to {time} seconds");
                        break;
                    default:
                        time = 3f;
                        GameLog.Instance.PostMessageToLog($"Step delay changed to {time} seconds");
                        break;
                }
                heistEvent.StepDelayTime = time;
                break;
            case "button3":
                logComponent.enabled = !logComponent.enabled;
                break;
        }
    }

    /*public event EventHandler<(string menuName, bool isChild, object passInfo)> LoadMenu;

    private void CallLoadMenu(string menuName, bool isChild, object passInfo)
    {
        LoadMenu.Invoke(this, (menuName, isChild, passInfo));
    }

    public event EventHandler<object> UnloadMenu;

    private void CallUnloadMenu(object passInfo)
    {
        UnloadMenu.Invoke(this, passInfo);
    }*/

    public void UnregisterCallbacks()
    {
        GameLog.Instance.LogUpdated -= UpdateLogDisplay;
        button1.UnregisterCallback<ClickEvent>(OnClick);
        button2.UnregisterCallback<ClickEvent>(OnClick);
        button3.UnregisterCallback<ClickEvent>(OnClick);
    }

    public void Update() { }

    public void SendMenuNewInfo(object info) {
        if (info.GetType() != typeof(EventController)) return;
        heistEvent = info as EventController;
        if (statusDisplay.text.Length > 0) statusDisplay.text = "";
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
                        //CallLoadMenu("SafehouseMenu", false, heistEvent.GetEventState());
                        break;
                    case HEventState.HeistFinished:
                        CallLoadMenu("SafehouseMenu", false, heistEvent.GetEventState());
                        return;
                }
                break;
            case HEventType.HType.Cmbt_Combat:
                // TODO: if this HeistHUD is going to handle returning to SafehouseMenu then it needs to be more robust
                switch (heistEvent.GetEventState())
                {
                    case HEventState.Begin:
                        break;
                    case HEventState.DoneFailure:
                        //CallLoadMenu("SafehouseMenu", false, heistEvent.GetEventState());
                        break;
                    case HEventState.DoneSuccess:
                        break;
                    case HEventState.HeistFinished:
                        CallLoadMenu("SafehouseMenu", false, heistEvent.GetEventState());
                        return;
                }

                CombatEvent e = heistEvent.BaseEvent as CombatEvent;
                if (e.CombatRound == null) break;
                if (e.CombatRound.AllCombatActors == null) break;
                StringBuilder sb = new();
                List<CrewMemberController> allActors =  e.CombatRound.AllCombatActors;
                foreach (CrewMemberController c in allActors)
                {
                    sb.AppendLine(c.ToShortString());
                }
                statusDisplay.text = sb.ToString();
                break;
        }
    }
}
