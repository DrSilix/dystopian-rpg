using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrewMemberOptions : MonoBehaviour
{
    public UIDocument uiDoc;
    public UIDocument attributesDoc;

    private Button attributesButton;
    private Button returnButton;

    public void InitializeMenu()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        returnButton = rootElem.Q("return") as Button;
        returnButton.RegisterCallback<ClickEvent>(OnClick);

        attributesButton = rootElem.Q("attributes") as Button;
        attributesButton.RegisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
    {
        Debug.Log(((VisualElement)e.currentTarget).name);
        switch (((VisualElement)e.currentTarget).name)
        {
            case "attributes":
                attributesDoc.enabled = true;
                attributesDoc.gameObject.GetComponent<CrewAttributesMenu>().InitializeMenu(1);
                break;
            case "return":
                UnregisterCallbacks();
                uiDoc.enabled = false;
                break;
        }
    }

    private void UnregisterCallbacks()
    {
        returnButton.UnregisterCallback<ClickEvent>(OnClick);
        attributesButton.UnregisterCallback<ClickEvent>(OnClick);
    }
}
