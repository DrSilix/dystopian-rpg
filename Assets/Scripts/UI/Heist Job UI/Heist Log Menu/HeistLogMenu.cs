using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HeistLogMenu : DefaultLoadUnloadMenuBehaviour, IMenu
{
    private HeistLog heistLog;

    private VisualElement scrollableContentContainer;
    private VisualElement[] itemVisualElements;
    private VisualTreeAsset entryPanel;

    private Button confirmButton;
    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        confirmButton = rootElem.Q("confirm") as Button;

        scrollableContentContainer = rootElem.Q("unity-content-container");

        scrollableContentContainer.Clear();

        entryPanel = Storyteller.Instance.UIController.GetVisualTreeAssetByName("HeistLogEntryPanel");

        heistLog = Storyteller.Instance.HeistController.Log;
        itemVisualElements = new VisualElement[heistLog.Log.Count];
        var i = 0;
        foreach (HeistLogEntry logEntry in heistLog.Log)
        {
            VisualElement itemElement = entryPanel.CloneTree();
            VisualElement containerColor = itemElement.Q("container-color");
            VisualElement icon = itemElement.Q("icon");
            Label textContainer = itemElement.Q("text") as Label;
            itemElement.tooltip = logEntry.StepNumber.ToString();
            Color backgroundColor = logEntry.EntryColor;
            backgroundColor.a = 0.1f;
            containerColor.style.backgroundColor = new StyleColor(backgroundColor);
            textContainer.text = $"{logEntry.GetHierarchyLevel()}><b>{logEntry.StepNumber}- {logEntry.EntryStartTime.ToShortTimeString()}.</b> {logEntry.ShortDescription}\n<size=22>{logEntry.Body}";

            scrollableContentContainer.Add(itemElement);
            itemVisualElements[i] = itemElement;
            i++;
        }
    }

    public void RegisterCallbacks()
    {
        foreach (VisualElement element in itemVisualElements)
        {
            element.RegisterCallback<ClickEvent>(OnClick);
        }
        confirmButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void SendMenuNewInfo(object info) { }

    public void UnregisterCallbacks()
    {
        foreach (VisualElement element in itemVisualElements)
        {
            element.UnregisterCallback<ClickEvent>(OnClick);
        }
        confirmButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        VisualElement target = ((VisualElement)e.currentTarget);
        Debug.Log(target.name);
        switch (target.name)
        {
            case "item-container":
                break;
            case "confirm":
                CallUnloadMenu(null);
                break;
        }
    }

    public void Update() { }
}
