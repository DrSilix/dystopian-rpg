using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument uiDoc;
    public EventSystem eventSystem;
    public GameObject safeHouseMenu;

    // private characterUIElements character1, character2, character3;
    private VisualElement startButton;


    // Start is called before the first frame update
    void Start()
    {
        VisualElement rootElem = uiDoc.rootVisualElement;
        /*character1 = new characterUIElements("container-character-1", rootElem);
        character2 = new characterUIElements("container-character-2", rootElem);
        character3 = new characterUIElements("container-character-3", rootElem);

        character1.RegisterCallbacks();
        character2.RegisterCallbacks();
        character3.RegisterCallbacks();*/

        startButton = rootElem.Q("game-start");
        startButton.RegisterCallback<ClickEvent>(OnStartClick);

        /*List<string> names = new List<string>();
        var stream = new StreamReader("Assets\\Data\\names.txt");
        while (!stream.EndOfStream)
        {
            names.Add(stream.ReadLine());
        }

        character1.name.value = names[Random.Range(0, names.Count)];
        character2.name.value = names[Random.Range(0, names.Count)];
        character3.name.value = names[Random.Range(0, names.Count)];*/


    }

    private void OnStartClick(ClickEvent e)
    {
        startButton.UnregisterCallback<ClickEvent>(OnStartClick);
        this.gameObject.SetActive(false);
        //safeHouseMenu.SetActive(true);
    }

    /*private void OnDisable()
    {
        character1.DeregisterCallbacks();
        character2.DeregisterCallbacks();
        character3.DeregisterCallbacks();
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public class characterUIElements
    {
        public VisualElement root;

        public TextField name;
        public TextField alias;
        public DropdownField race;
        public DropdownField gameClass;

        public ProgressBar health;
        public Label defense;
        public Label initiative;

        public SliderInt body;
        public SliderInt agility;
        public SliderInt reaction;
        public SliderInt strength;

        public SliderInt willpower;
        public SliderInt logic;
        public SliderInt intuition;
        public SliderInt charisma;

        public SliderInt luck;

        public DropdownField pistol;
        public DropdownField armor;

        public characterUIElements(string rootId, VisualElement topRoot)
        {
            root = topRoot.Q(rootId) as VisualElement;
            
            name = root.Q("character-name") as TextField;
            alias = root.Q("character-alias") as TextField;
            race = root.Q("character-race") as DropdownField;
            gameClass = root.Q("character-class") as DropdownField;

            health = root.Q("character-health") as ProgressBar;
            defense = root.Q("character-defense") as Label;
            initiative = root.Q("character-initiative") as Label;

            body = root.Q("character-body") as SliderInt;
            agility = root.Q("character-agility") as SliderInt;
            reaction = root.Q("character-reaction") as SliderInt;
            strength = root.Q("character-strength") as SliderInt;

            willpower = root.Q("character-will") as SliderInt;
            logic = root.Q("character-logic") as SliderInt;
            intuition = root.Q("character-intuition") as SliderInt;
            charisma = root.Q("character-charisma") as SliderInt;

            luck = root.Q("character-luck") as SliderInt;

            pistol = root.Q("character-pistol") as DropdownField;
            armor = root.Q("character-armor") as DropdownField;
        }

        public void RegisterCallbacks() {
            name.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            alias.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            race.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            gameClass.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            body.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            agility.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            reaction.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            strength.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            willpower.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            logic.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            intuition.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            charisma.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            luck.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            pistol.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
            armor.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
        }

        public void DeregisterCallbacks()
        {
            name.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            alias.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            race.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            gameClass.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            body.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            agility.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            reaction.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            strength.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            willpower.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            logic.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            intuition.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            charisma.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            luck.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            pistol.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
            armor.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
        }

        private void OnChangeEvent(ChangeEvent<string> e)
        {
            Debug.Log($"{e.newValue} -> {e.target}");
        }
    }*/

    public enum Race { Human, Elf, Orc, Dwarf, Troll};
    public enum GameClass { Enforcer, Hacker, Face }
}
