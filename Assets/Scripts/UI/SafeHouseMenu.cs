using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SafeHouseMenu : MonoBehaviour
{
    public UIDocument uiDoc;
    public UIDocument crewUIDoc;
    public EventSystem eventSystem;
    public WorldController worldController;
    public GameObject hideoutImage;


    private Button beginButton;
    private Button crewButton;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        beginButton = rootElem.Q("game-begin") as Button;
        beginButton.RegisterCallback<ClickEvent>(OnClick);

        crewButton = rootElem.Q("crew") as Button;
        crewButton.RegisterCallback<ClickEvent>(OnClick);
    }

    private void OnClick(ClickEvent e)
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
                crewUIDoc.enabled = true;
                crewUIDoc.gameObject.GetComponent<CrewMenu>().InitializeMenu();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
