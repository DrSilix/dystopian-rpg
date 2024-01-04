using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SafeHouseMenu : IMenu
{
    public EventSystem eventSystem;
    public WorldController worldController;
    public GameObject hideoutImage;

    private Button beginButton;
    private Button crewButton;

    private VisualElement bgImage;
    private float bgScrollSpeed = -4;
    private bool initialized;
    private int initWait;

    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        beginButton = rootElem.Q("game-begin") as Button;
        crewButton = rootElem.Q("crew") as Button;

        bgImage = rootElem.Q("bg-image");

        initialized = true;
    }

    public void RegisterCallbacks()
    {
        beginButton.RegisterCallback<ClickEvent>(OnClick);
        crewButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void UnregisterCallbacks()
    {
        beginButton.UnregisterCallback<ClickEvent>(OnClick);
        crewButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    /*private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "game-begin":
                beginButton.UnregisterCallback<ClickEvent>(OnClick);
                this.gameObject.SetActive(false);
                hideoutImage.SetActive(false);
                worldController.StartLevel();
                break;
            case "crew":
                //crewUIDoc.enabled = true;
                //crewUIDoc.gameObject.GetComponent<CrewMenu>().InitializeMenu();
                break;
        }
    }*/

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "game-begin":
                CallLoadMenu("HeistHUD", false, null);
                Storyteller.Instance.StartHeist();
                break;
            case "crew":
                CallLoadMenu("CrewMenu", true, null);
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

    public void Update()
    {
        if (!initialized) return;
        Vector3 prevOffset = bgImage.transform.position;
        float width = bgImage.resolvedStyle.width;
        float parentWidth = bgImage.parent.resolvedStyle.width;

        if (prevOffset.x < -(width - parentWidth) || prevOffset.x > 0) bgScrollSpeed = -bgScrollSpeed;
        bgImage.transform.position = prevOffset + (Vector3.left * bgScrollSpeed * Time.deltaTime);
    }
}
