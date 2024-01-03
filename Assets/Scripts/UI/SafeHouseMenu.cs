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
    public float speed = 1;


    private Button beginButton;
    private Button crewButton;

    private VisualElement bgImage;
    private bool _initialized;
    private int initWait;

    void InitializeMenu()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;

        beginButton = rootElem.Q("game-begin") as Button;
        beginButton.RegisterCallback<ClickEvent>(OnClick);

        crewButton = rootElem.Q("crew") as Button;
        crewButton.RegisterCallback<ClickEvent>(OnClick);

        bgImage = rootElem.Q("bg-image");
        bgImage.transform.position += Vector3.left;
        Debug.Log(bgImage.resolvedStyle.width);

        speed *= -1;

        _initialized = true;
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

    void FixedUpdate()
    {
        if (initWait < 1)
        {
            initWait++;
            return;
        }
        if (!_initialized) InitializeMenu();
        Vector3 prevOffset = bgImage.transform.position;
        float width = bgImage.resolvedStyle.width;
        float parentWidth = bgImage.parent.resolvedStyle.width;

        if (prevOffset.x < -(width - parentWidth) || prevOffset.x > 0) speed = -speed;
        bgImage.transform.position = prevOffset + (Vector3.left * speed * Time.deltaTime);
        //material.SetTextureOffset("_MainTex", prevOffset + (Vector2.right * speed * Time.deltaTime));
    }
}
