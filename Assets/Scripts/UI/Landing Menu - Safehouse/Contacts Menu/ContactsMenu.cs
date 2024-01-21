using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContactsMenu : IMenu
{
    private List<ContactSO> contacts;

    private VisualElement scrollableContentContainer;
    private VisualElement[] contactVisualElements;

    private Button safeHouseButton;
    public void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        contacts = Storyteller.Instance.ContactSOs;

        VisualElement rootElem = uiDoc.rootVisualElement;

        safeHouseButton = rootElem.Q("safehouse") as Button;

        scrollableContentContainer = rootElem.Q("unity-content-container");

        scrollableContentContainer.Clear();

        contactVisualElements = new VisualElement[contacts.Count];
        for (int i = 0; i < contacts.Count; i++)
        {
            VisualElement[] contactElement = GetBlankContactTemplate();
            var c = contacts[i];
            contactElement[0].tooltip = c.displayName;
            ((Label)contactElement[1]).text = "\u2706";
            ((Label)contactElement[2]).text = c.displayName;
            ((Label)contactElement[3]).text = $"<b>Type:</b> Fixer\t<b>Relation:</b>{c.relation}  <b>Favors:</b>{c.favors}  <b>Influence:</b>{c.influence}";

            scrollableContentContainer.Add(contactElement[0]);
            contactVisualElements[i] = contactElement[0];
        }

    }

    public VisualElement[] GetBlankContactTemplate()
    {
        var root = new VisualElement();
        root.name = "contact-container";
        var phoneIconContainer = new VisualElement();
        root.Add(phoneIconContainer);

        var phoneIcon = new Label();
        phoneIcon.name = "phone-icon";
        phoneIconContainer.Add(phoneIcon);

        var infoContainer = new VisualElement();
        root.Add(infoContainer);

        var name = new Label();
        var info = new Label();
        name.name = "name";
        info.name = "info";
        infoContainer.Add(name);
        infoContainer.Add(info);

        root.AddToClassList("contact-container");
        phoneIconContainer.AddToClassList("phone-icon-container");
        phoneIcon.AddToClassList("phone-icon");
        infoContainer.AddToClassList("info-container");
        name.AddToClassList("contact-name");

        return new VisualElement[] { root, phoneIcon, name, info };
    }

    public void RegisterCallbacks()
    {
        foreach (VisualElement element in contactVisualElements)
        {
            element.RegisterCallback<ClickEvent>(OnClick);
        }
        safeHouseButton.RegisterCallback<ClickEvent>(OnClick);
    }

    public void SendMenuNewInfo(object info) { }

    public void UnregisterCallbacks()
    {
        foreach (VisualElement element in contactVisualElements)
        {
            element.UnregisterCallback<ClickEvent>(OnClick);
        }
        safeHouseButton.UnregisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        VisualElement target = ((VisualElement)e.currentTarget);
        Debug.Log(target.name);
        switch (target.name)
        {
            case "contact-container":
                CallLoadMenu("ContactOptions", true, target.tooltip);
                break;
            case "safehouse":
                CallLoadMenu("SafehouseMenu", false, null);
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

    public void Update() { }
}
