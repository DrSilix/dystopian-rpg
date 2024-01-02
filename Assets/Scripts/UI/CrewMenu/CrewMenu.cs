using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CrewMenu : MonoBehaviour
{
    public UIDocument uiDoc;
    public UIDocument crewMemberOptionsDoc;

    private VisualElement crewMember1, crewMember2, crewMember3;
    private Button doneButton;

    /*public void OnEnable()
    {
        InitializeMenu();
    }*/

    public void InitializeMenu()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        doneButton = rootElem.Q("done-button") as Button;
        doneButton.RegisterCallback<ClickEvent>(OnClick);

        crewMember1 = rootElem.Q("crew-member-1");
        crewMember1.RegisterCallback<ClickEvent>(OnClick);
        crewMember2 = rootElem.Q("crew-member-2");
        crewMember2.RegisterCallback<ClickEvent>(OnClick);
        crewMember3 = rootElem.Q("crew-member-3");
        crewMember3.RegisterCallback<ClickEvent>(OnClick);

        UpdateCrewMemberInfo(crewMember1, 1);
        UpdateCrewMemberInfo(crewMember2, 2);
        UpdateCrewMemberInfo(crewMember3, 3);
    }

    private void UpdateCrewMemberInfo(VisualElement crewMember, int crewId)
    {
        CrewMemberController memberController = Storyteller.Instance.Crew.GetCrewMember(crewId);

        Label title = crewMember.Q("title-bar") as Label;
        title.text = memberController.alias + " - Lvl 4 - ";

        VisualElement icon = crewMember.Q("icon");
        Label[] attributes = new Label[8];
        Label[] stats = new Label[4];
        
        for (int i = 0; i < 8; i++)
        {
            attributes[i] = crewMember.Q(((Attribute)i).ToString() + "-value") as Label;
            attributes[i].text = memberController.GetAttribute((Attribute)i).ToString();
        }
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "crew-member-1":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "crew-member-2":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "crew-member-3":
                crewMemberOptionsDoc.enabled = true;
                crewMemberOptionsDoc.gameObject.GetComponent<CrewMemberOptions>().InitializeMenu();
                break;
            case "done-button":
                UnregisterCallbacks();
                uiDoc.enabled = false;
                break;
        }
    }

    private void UnregisterCallbacks()
    {
        doneButton.UnregisterCallback<ClickEvent>(OnClick);
        crewMember1.UnregisterCallback<ClickEvent>(OnClick);
        crewMember2.UnregisterCallback<ClickEvent>(OnClick);
        crewMember3.UnregisterCallback<ClickEvent>(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
