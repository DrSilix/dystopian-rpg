using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SafeHouseMenu : MonoBehaviour
{
    public UIDocument uiDoc;
    public EventSystem eventSystem;
    public WorldController worldController;
    public GameObject hideoutImage;

    private Button beginButton;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        beginButton = rootElem.Q("game-begin") as Button;
        beginButton.SetEnabled(false);
        beginButton.RegisterCallback<ClickEvent>(OnBeginClick);
    }

    private void OnBeginClick(ClickEvent e)
    {
        beginButton.UnregisterCallback<ClickEvent>(OnBeginClick);
        this.gameObject.SetActive(false);
        hideoutImage.SetActive(false);
        worldController.StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
