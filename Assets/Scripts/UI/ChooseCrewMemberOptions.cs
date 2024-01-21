using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChooseCrewMemberOptions : MenuOptionsBase, IMenu
{
    List<CrewMemberController> crewMembers;
    public override void InitializeMenu(UIDocument uiDoc, object passInfo)
    {
        base.InitializeMenu(uiDoc, passInfo);

        crewMembers = Storyteller.Instance.Crew.CrewMembers;
        ((Button)Buttons[0]).text = crewMembers[0].alias;
        ((Button)Buttons[1]).text = crewMembers[1].alias;
        ((Button)Buttons[2]).text = crewMembers[2].alias;
    }

    public override void OnClick(ClickEvent e)
    {
        string targetName = ((VisualElement)e.currentTarget).name;
        Debug.Log(targetName);

        switch (targetName)
        {
            case "crew-member-1":
                CallUnloadMenu(crewMembers[0], false);
                break;
            case "crew-member-2":
                CallUnloadMenu(crewMembers[1], false);
                break;
            case "crew-member-3":
                CallUnloadMenu(crewMembers[2], false);
                break;
            case "return":
                CallUnloadMenu(null);
                break;
        }
    }
}
