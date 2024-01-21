using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class MenuOptionsBase : IMenu
{
    public List<VisualElement> Buttons { get; set; }

    private object passInfo;

    public virtual void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        this.passInfo = passInfo;

        VisualElement rootElem = uiDoc.rootVisualElement;
        VisualElement buttonContainer = rootElem.Q("button-container");

        Buttons = buttonContainer.Children().ToList();
    }

    public virtual void RegisterCallbacks()
    {
        foreach (var button in Buttons)
        {
            button.RegisterCallback<ClickEvent>(OnClick);
        }
    }

    public virtual void OnClick(ClickEvent e)
    {
        string targetName = ((VisualElement)e.currentTarget).name;
        Debug.Log(targetName);        
        if (targetName == "return")
        {
            CallUnloadMenu(null);
            return;
        }
        CallLoadMenu(targetName, true, passInfo);
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

    public virtual void UnregisterCallbacks()
    {
        foreach (var button in Buttons)
        {
            button.UnregisterCallback<ClickEvent>(OnClick);
        }
    }

    public virtual void Update() { }
    public virtual void SendMenuNewInfo(object info) { }
}
