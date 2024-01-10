/*
 * Architect of all, persists accross individual heists
 * Maintains awareness of all game mechanic states (events, crew, enemies, etc.)
 * Contains story information (written and abstract(checks on how the antagonist reacts)) and handles invoking that in story scenes and overarching actions (a guiding hand to the protagonist)
 * 
 * is this the protagonist director?
 * or does it control the protagonist and antagonist directors?
 * 
 * 
 * player director (watches over players and decides next steps on paradigm change)
 * enemy director (oblivious, gets fed events (alarms, etc.) triggers reactions)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UIElements;

public class Storyteller : MonoBehaviour
{
    [SerializeField] private GameObject crewPrefab;
    [SerializeField] private GameObject crewMember1Prefab, crewMember2Prefab, crewMember3Prefab;
    [SerializeField] private GameObject enemyCrewPrefab;
    [SerializeField] private GameObject enemyCrewMember1Prefab, enemyCrewMember2Prefab;
    [SerializeField] private WorldController worldController;
    [SerializeField] private AssetLabelReference assetLabelRef;

    public Dictionary<string, WeaponSO> WeaponSOs {  get; private set; } = new Dictionary<string, WeaponSO>();
    public Dictionary<string, ArmorSO> ArmorSOs { get; private set; } = new Dictionary<string, ArmorSO>();
    public Dictionary<string, AmmunitionSO> AmmunitionSOs { get; private set; } = new Dictionary<string, AmmunitionSO>();
    
    public CrewController Crew {  get; private set; }
    
    
    public static Storyteller Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        InitializeInstance();
    }

    private async void InitializeInstance()
    {
        await Addressables.LoadAssetsAsync<object>(assetLabelRef, (a) =>
        {
            switch (a)
            {
                case WeaponSO s:
                    WeaponSOs.Add(s.displayName, s);
                    break;
                case AmmunitionSO s:
                    AmmunitionSOs.Add(s.displayName, s);
                    break;
                case ArmorSO s:
                    ArmorSOs.Add(s.displayName, s);
                    break;
            }
        }).Task;
        GenerateCrew();
        worldController.GenerateLevel();
    }

    public void StartHeist()
    {
        worldController.StartLevel();
    }

    // TODO: subscribable event HeistEventStateChange
    // TODO: public get for current running event name

    public delegate void HeistEventStateChanged(EventController baseEvent);
    public HeistEventStateChanged heistEventStateChanged;

    private void GenerateCrew()
    {
        GameObject crewGO = Instantiate(crewPrefab, Vector3.zero, Quaternion.identity);
        Crew = crewGO.GetComponent<CrewController>();
        GameObject crewMember1 = Instantiate(crewMember1Prefab, crewGO.transform);
        GameObject crewMember2 = Instantiate(crewMember2Prefab, crewGO.transform);
        GameObject crewMember3 = Instantiate(crewMember3Prefab, crewGO.transform);
        Crew.AddCrewMembers(crewMember1, crewMember2, crewMember3);
    }

    public CrewController GenerateEnemies(int numberOfEnemies)
    {
        GameObject crewGO = Instantiate(enemyCrewPrefab, Vector3.zero, Quaternion.identity);
        CrewController crew = crewGO.GetComponent<CrewController>();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject crewMember = Instantiate(enemyCrewMember1Prefab, crewGO.transform);
            char letter = 'A';
            CrewMemberController enemy = crewMember.GetComponent<CrewMemberController>();
            enemy.alias = $"Guard {(char)(letter + i)}";
            int temp = enemy.attributes.Set(Attribute.body, Random.Range(4, 7));
            enemy.attributes.Set(Attribute.agility, 6 - temp + 4);
            enemy.tempWeaponSkillValue = Random.Range(8, 13);
            Weapon weapon = (Random.Range(1, 3) == 1) ? new Weapon(WeaponSOs["Vyner S-12"]) : new Weapon(WeaponSOs["Cobalt Defender"]);
            enemy.EquippedItems.Equip(weapon);
            Armor armor = (Random.Range(1, 3) == 1) ? new Armor(ArmorSOs["Guard Outfit"]) : new Armor(ArmorSOs["Bullet Proof Vest"]);
            enemy.EquippedItems.Equip(armor);
            crew.AddCrewMember(crewMember);
        }
        return crew;
    }
}
