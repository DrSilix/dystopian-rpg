using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

/// <summary>
/// Handles all actions/data related to an individual crew member
/// </summary>
public partial class CrewMemberController : MonoBehaviour
{
    private CrewController crewController;
    
    public string alias;
    [field: SerializeField] public Sprite Icon {  get; set; }
    [field: SerializeField] public int MaxDamage { get; private set; }
    public Attributes attributes;
    [field: SerializeField] public int InitiativeModifier { get; private set; }
    [field: SerializeField] public int InitiativeDice { get; private set; } = 1;
    [SerializeField] private bool isEnemy;
    [Header("DefaultEquipment")]
    public WeaponSO defaultWeapon;
    public ArmorSO defaultArmor;

    public EquippedItems EquippedItems { get; private set; }
    public bool IsEnemy { get { return isEnemy; } private set { isEnemy = value; } }

    public int tempWeaponSkillValue;

    //public int CurrentDamagedAmount { get; private set; }
    public CombatState CurrentCombatState { get; private set; }

    /// 0 = undamaged / 1 = damaged / 2 = critical / 3 = downed / 4 = dead
    public DamagedState CurrentDamagedState { get => GetDamagedState(); }
    public int DamageTaken { get; private set; }

    // TODO: fix just leaving values in here
    private List<object> combatStoredValues;

    /// <summary>
    /// Initialize is to be called after the crew is initialized and the crew member is added to it.
    /// Creates the default weapons and armor (debug), adds them to the inventory, and then equips them.
    /// Calculates initiative and health
    /// </summary>
    public void Initialize()
    {
        Inventory inv = crewController.CrewInventory;
        InventoryItem w = inv.Add(new Weapon(defaultWeapon));
        InventoryItem a = inv.Add(new Armor(defaultArmor));
        EquippedItems = new EquippedItems(w, a, this);
        // TODO: these need to be recalculated when the relevant attributes change. Possibly move them back into Attributes
        InitiativeModifier = attributes.intuition + attributes.reaction;
        MaxDamage = 8 + Mathf.CeilToInt((float)attributes.body / 2);
    }

    public void ResetToFull()
    {
        DamageTaken = 0;
        EquippedItems.EquippedWeapon.Reload();
    }

    /// <summary>
    /// Addes IInventoryItem such as Weapon/Armor to inventory and equips it to slot
    /// </summary>
    /// <param name="slot">ItemSlot where item will be equipped</param>
    /// <param name="item">Item to be equipped</param>
    public void AddToInventoryAndEquipItem(EquippedItems.ItemSlot slot, IInventoryItem item)
    {
        InventoryItem invItem = crewController.CrewInventory.Add(item);
        EquippedItems.Equip(slot, invItem);
    }

    /// <summary>
    /// Adds a reference to the crew that this crew member is attached to
    /// </summary>
    /// <param name="controller"></param>
    public void SetConnectedCrew(CrewController controller) { crewController = controller; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{alias}|{CurrentDamagedState}|{DamageTaken}/{MaxDamage}|{(isEnemy ? "Enemy" : "Player")}###");
        sb.Append($"{attributes}###");
        sb.Append($"{EquippedItems.EquippedWeapon}###");
        sb.Append($"{EquippedItems.EquippedArmor}");
        return sb.ToString();
    }
    public string ToShortString()
    {
        StringBuilder sb = new();
        sb.Append($"{alias}|{CurrentDamagedState}|{DamageTaken}/{MaxDamage}##");
        sb.Append($"{EquippedItems.EquippedWeapon.ToShortString()}##");
        sb.Append($"{EquippedItems.EquippedArmor.ToShortString()}");
        return sb.ToString();
    }

    /// <summary>
    /// Gets the value of the crew members attribute
    /// </summary>
    /// <param name="attribute">attribute to get</param>
    /// <returns>value of attribute</returns>
    public int GetAttribute(Attribute attribute) { return attributes.Get(attribute); }

    /// <summary>
    /// Gets the number of successes for a simple roll of the two attributes. The two attributes are summed with the modifier
    /// and that is how many dice are rolled
    /// </summary>
    /// <param name="attribute1">Primary attribute to use</param>
    /// <param name="attribute2">Secondary Attribute to use</param>
    /// <param name="modifier">Added directly to number of dice to roll</param>
    /// <returns>Number of successes for the roll</returns>
    public int GetAttributeRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Basic(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }
    /// <summary>
    /// Gets the successes and critical result for an advanced roll using two attributes. The two attributes are summed witht he modifier
    /// and that is how many dice are rolled. A crit is more than half the dice being successes,
    /// a critical failure is more than half being a failure (roll a 1) and no successes
    /// </summary>
    /// <param name="attribute1">Primary attribute to use</param>
    /// <param name="attribute2">Secondary Attribute to use</param>
    /// <param name="modifier">Added directly to number of dice to roll</param>
    /// <returns>A tuple containing the number of successes for the roll along with the crit value (1 for crit, -1 for crit fail, 0 default)</returns>
    public (int successes, int crit) GetAttributeAdvancedRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Adv(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }

    /// <summary>
    /// Rolls the crew members initiative using a more traditional dice total plus the modifier
    /// </summary>
    /// <returns>the rolled initiative</returns>
    public int RollInitiative()
    {
        return InitiativeModifier + Roll.Initiative(InitiativeDice);
    }

    // TODO: move this to a separate class, that this inherits??
    #region Combat
    public void CreateCombatState()
    {
        CurrentCombatState = new();
    }

    /// <summary>
    /// Damaged state based on the amount of damage taken compared to the total health
    /// no damage = healthy, between no and half damage = wounded, between half and full damage = critical, fully damaged = bleeding out, fully damaged + 4 = dead
    /// </summary>
    /// <returns>Damaged state</returns>
    public DamagedState GetDamagedState()
    {
        int halfHealth = Mathf.FloorToInt((float)MaxDamage / 2);
        // after being downed at n/n there are 3 rounds of 1 damage "bleeding out" and then the 4th round is death
        if (DamageTaken > MaxDamage + 3) return DamagedState.Dead;
        else if (DamageTaken >= MaxDamage) return DamagedState.BleedingOut;
        else if (DamageTaken > halfHealth) return DamagedState.Critical;
        else if (DamageTaken > 0) return DamagedState.Wounded;
        return DamagedState.Healthy;
    }

    public CombatAction ChooseCombatAction()
    {
        if (EquippedItems.EquippedWeapon.CurrentAmmoCount <= 0) return CombatAction.Reload;
        return CombatAction.Attack;
    }

    /// <summary>
    /// Primary combat logic where the AI chooses the highest priority action+target out of all action+targets
    /// </summary>
    /// <param name="allActors">All other combat actors involved</param>
    /// <param name="selectRandomly">whether to select the target randomly (used for enemies)</param>
    /// <returns>a tuple containing the CombatAction and CrewMemberController target</returns>
    public (CombatAction, CrewMemberController) ChooseActionAndTarget(List<CrewMemberController> allActors, bool selectRandomly)
    {
        combatStoredValues = new();
        // incomplete, this just chooses attack unless out of ammo
        CombatAction combatAction = ChooseCombatAction();
        CrewMemberController combatTarget = this;

        switch (combatAction)
        {
            // grabs all enemies (relative) that are healthier than bleeding out and orders them randomly
            // the loops through them selecting the first most damaged state enemy. If this is an actual player enemy
            // then overrides with random selection if selectRandomly
            case CombatAction.Attack:
                List<CrewMemberController> enemyTargets = (from item in allActors
                                                           where item.IsEnemy != IsEnemy
                                                            && item.CurrentDamagedState < DamagedState.BleedingOut
                                                           select item).OrderBy(a => Random.value).ToList();
                combatTarget = enemyTargets[0];
                foreach (CrewMemberController member in enemyTargets) if (member.CurrentDamagedState < DamagedState.BleedingOut && member.CurrentDamagedState > combatTarget.CurrentDamagedState) combatTarget = member;
                if (selectRandomly) combatTarget = enemyTargets[Random.Range(0, enemyTargets.Count)];
                GameLog.Instance.PostMessageToLog($"{this.alias} takes aim with their {this.EquippedItems.EquippedWeapon.DisplayName} {this.EquippedItems.EquippedWeapon.WeaponType}");
                break;
            case CombatAction.Reload:
                combatTarget = this;
                GameLog.Instance.PostMessageToLog($"{this.alias} is out of ammo");
                break;
        }
        return (combatAction, combatTarget);
    }

    /// <summary>
    /// Takes the first of two sub-steps to perform and resolve the combat action (generic, not necessarily attack)
    /// </summary>
    /// <param name="combatAction">Action to take</param>
    /// <param name="target">Target to take action on</param>
    public void BeginPerformCombatActionOnTarget (CombatAction combatAction, object target)
    {
        switch (combatAction)
        {
            // uses combat similar(if not same) to shadowrun. attacker roll to see if hit, defender rolls to see if dodge/miss
            // the results are persisted in the combatStoredValues list of objects
            case CombatAction.Attack:
                int targetCover = 4;
                CrewMemberController combatTarget = (CrewMemberController)target;
                // shot to-hit value
                (int shotToHitValue, int defenseModifier) = this.FireWeaponToHit(0);
                // target to-evade value
                int targetToEvadeValue = (combatTarget).DefendAgainstWeaponAttack(defenseModifier + targetCover);
                combatStoredValues.Add(shotToHitValue);
                combatStoredValues.Add(targetToEvadeValue);
                GameLog.Instance.PostMessageToLog($"{this.alias} shot at {combatTarget.alias} rolling {shotToHitValue} to hit vs. {targetToEvadeValue}");
                break;
            case CombatAction.Reload:
                Weapon weapon = this.EquippedItems.EquippedWeapon;
                GameLog.Instance.PostMessageToLog($"{this.alias} is reloading their {weapon.DisplayName}");
                //Reloading
                break;
        }
    }

    /// <summary>
    /// Takes the second of two sub-steps to perform and resolve the combat action (generic, not necessarily attack)
    /// </summary>
    /// <param name="combatAction">Action to take</param>
    /// <param name="target">Target to take action on</param>
    public void ResolvePerformCombatActionOnTarget(CombatAction combatAction, object target)
    {
        switch (combatAction)
        {
            // uses shadowrun style combat. Net hits are recorded (tohit - toevade), if 0 or less then miss.
            // if positive net hits then calculate damage (no roll) net hits + weapon damage
            // then defender rolls to resist the modified damage value, if the damage value is greater then damage is applied
            case CombatAction.Attack:
                int shotToHitValue = (int)combatStoredValues[0], targetToEvadeValue = (int)combatStoredValues[1];
                CrewMemberController combatTarget = target as CrewMemberController;
                // missed
                if (shotToHitValue <= targetToEvadeValue)
                {
                    GameLog.Instance.PostMessageToLog($"{this.alias} missed {combatTarget.alias}");
                    break;
                }
                // get damage and then have target resist damage
                int modifiedDamageValue = this.DetermineDamageDealt(shotToHitValue - targetToEvadeValue);
                int enemyDamageToTake = combatTarget.RollToResistDamage(modifiedDamageValue, this.EquippedItems.EquippedWeapon.ArmorPiercing);
                // failed to absorb all damage
                if (enemyDamageToTake > 0)
                {
                    GameLog.Instance.PostMessageToLog($"{this.alias} hit with {modifiedDamageValue} dmg. {combatTarget.alias} takes {enemyDamageToTake} dmg!");
                    if (combatTarget.CurrentDamagedState >= DamagedState.BleedingOut) GameLog.Instance.PostMessageToLog($"{combatTarget.alias} is down!");
                    combatTarget.TakeDamage(enemyDamageToTake);
                } else
                {
                    GameLog.Instance.PostMessageToLog($"{this.alias} hit with {modifiedDamageValue} dmg. {combatTarget.alias} resisted all damage!");
                }
                break;
            case CombatAction.Reload:
                Weapon weapon = this.EquippedItems.EquippedWeapon;
                GameLog.Instance.PostMessageToLog($"{this.alias} has reloaded {weapon.AmmoCapacity - weapon.CurrentAmmoCount} bullets");
                weapon.Reload();
                //Reloading
                break;
        }
    }

    /// <summary>
    /// "Fires" the weapon. calculates recoil. Fires a number of rounds determined by firingmode (subtracts from weapon ammo)
    /// adds those rounds as recoil and adds them to a defender defense modifier (more bullets = harder to defend against)
    /// </summary>
    /// <param name="modifier">To hit modifier, added directly to number of dice to roll</param>
    /// <returns>a tuple containing the roll result and the modification to defenders defence roll</returns>
    public (int, int) FireWeaponToHit(int modifier = 0)
    {
        Weapon weapon = EquippedItems.EquippedWeapon;
        int recoil = 1 + Mathf.CeilToInt((float)attributes.strength / 3) + weapon.Recoil;
        int defenseModifier = 0;
        int roundsToFire;
        switch (weapon.FiringMode)
        {
            case FiringMode.SingleShot:
            case FiringMode.SemiAuto:
                roundsToFire = 1;
                weapon.FireRounds(roundsToFire);
                recoil -= 1;
                break;
            case FiringMode.BurstFire:
                roundsToFire = Mathf.Min(weapon.CurrentAmmoCount, 3);
                weapon.FireRounds(roundsToFire);
                recoil -= roundsToFire;
                defenseModifier = -(roundsToFire - 1);
                break;
            case FiringMode.FullAuto:
                roundsToFire = Mathf.Min(weapon.CurrentAmmoCount, 6);
                weapon.FireRounds(roundsToFire);
                recoil -= roundsToFire;
                defenseModifier = -(roundsToFire - 1);
                break;
        }
        if (recoil > 0) recoil = 0;
        // a shadowrun d6 roll of the weapon skill + agility + modifier + recoil(negative) and the successes are bounded by the accuracy of the weapon
        return (Mathf.Min(Roll.Basic(tempWeaponSkillValue + attributes.agility + modifier + recoil), weapon.Accuracy), defenseModifier);
    }

    /// <summary>
    /// Simple shadowrun roll, reaction + intuition + modifier
    /// </summary>
    /// <param name="modifier">Added directly to the defense/evade roll</param>
    /// <returns>result of roll</returns>
    public int DefendAgainstWeaponAttack(int modifier = 0)
    {
        return Roll.Basic(attributes.reaction + attributes.intuition + modifier);
    }

    public int DetermineDamageDealt(int netHits)
    {
        return EquippedItems.EquippedWeapon.Damage + netHits;
    }

    /// <summary>
    /// After it is determined the attack "hit" then the defender resists with their body and armor
    /// weapon Armor piercing is added (negative value) to the armor beforehand
    /// </summary>
    /// <param name="damage">Damage to try to resist</param>
    /// <param name="ap">Armor Piercing value of the weapon (negative usually)</param>
    /// <returns>Amount of damage to take or -1 if no damage to take</returns>
    public int RollToResistDamage(int damage, int ap)
    {
        int armorVal = EquippedItems.EquippedArmor.ArmorRating + ap;
        if (damage > armorVal) return Roll.Basic(attributes.body + armorVal);
        return -1;
    }

    /// <summary>
    /// Addes the damage to the persistent crew member DamageTaken. Imporantly knows if the damage state changed and is verbose
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    /// <returns>Current damage state after applying damage</returns>
    public DamagedState TakeDamage(int damage)
    {
        DamagedState prevState = CurrentDamagedState;
        DamageTaken += damage;
        if (CurrentDamagedState != prevState) GameLog.Instance.PostMessageToLog($"{alias} is {CurrentDamagedState}");
        return CurrentDamagedState;
    }

    // TODO: am I using this or not?
    public class CombatState
    {
        public bool IsTakingTurn { get; set; } = false;
        public int ActionCount { get; set; } = 0;

    }

    #endregion
}
