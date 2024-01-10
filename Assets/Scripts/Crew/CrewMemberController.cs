using Assets.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class CrewMemberController : MonoBehaviour
{
    public string alias;
    public int MaxDamage { get; private set; }
    [SerializeField]
    public Attributes attributes;
    [SerializeField] private int initiativeModifier;
    [SerializeField] private int initiativeDice = 1;
    [SerializeField] private bool isEnemy;
    [Header("DefaultEquipment")]
    public WeaponSO defaultWeapon;
    public ArmorSO defaultArmor;

    public Equipped EquippedItems { get; private set; }
    public bool IsEnemy { get { return isEnemy; } private set { isEnemy = value; } }

    [SerializeField] public int tempWeaponSkillValue;

    //public int CurrentDamagedAmount { get; private set; }
    public CombatState CurrentCombatState { get; private set; }

    /// 0 = undamaged / 1 = damaged / 2 = critical / 3 = downed / 4 = dead
    public DamagedState CurrentDamagedState { get => GetDamagedState(); }
    public int DamageTaken { get; private set; }

    // TODO: fix just leaving values in here
    private List<object> combatStoredValues;

    void OnEnable()
    {
        EquippedItems = new Equipped(new Weapon(defaultWeapon), new Armor(defaultArmor));
        initiativeModifier = attributes.intuition + attributes.reaction;
        MaxDamage = 8 + Mathf.CeilToInt((float)attributes.body / 2);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"{alias}|{CurrentDamagedState}|{DamageTaken}/{MaxDamage}|{(isEnemy ? "Enemy" : "Player")}###");
        /*foreach(int combatVal in combatStoredValues)
        {
            sb.Append($"{combatVal}/");
        }
        sb.Remove(sb.Length - 1,sb.Length);
        sb.Append("\n");*/
        sb.Append($"{attributes}###");
        sb.Append($"{EquippedItems.EquippedWeapon}###");
        sb.Append($"{EquippedItems.EquippedArmor}");
        return sb.ToString();
    }

    public int GetAttribute(Attribute attribute) { return attributes.Get(attribute); }

    // public int GetSkillRoll(int skill) { }
    public int GetAttributeRoll (Attribute attribute, int modifier = 0)
    {
        return Roll.Basic(attributes.Get(attribute) + modifier);
    }
    public int GetAttributeRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Basic(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }
    public (int successes, int crit) GetAttributeAdvancedRoll(Attribute attribute1, Attribute attribute2, int modifier = 0)
    {
        return Roll.Adv(attributes.Get(attribute1) + attributes.Get(attribute2) + modifier);
    }

    public int RollInitiative()
    {
        return initiativeModifier + Roll.Initiative(initiativeDice);
    }

    #region Combat
    public void CreateCombatState()
    {
        CurrentCombatState = new();
    }

    public DamagedState GetDamagedState()
    {
        int halfHealth = Mathf.FloorToInt((float)MaxDamage / 2);
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

    public (CombatAction, CrewMemberController) ChooseActionAndTarget(List<CrewMemberController> allActors, bool selectRandomly)
    {
        combatStoredValues = new();
        CombatAction combatAction = ChooseCombatAction();
        CrewMemberController combatTarget = this;

        switch (combatAction)
        {
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

    public void BeginPerformCombatActionOnTarget (CombatAction combatAction, object target)
    {
        switch (combatAction)
        {
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

    public void ResolvePerformCombatActionOnTarget(CombatAction combatAction, object target)
    {
        switch (combatAction)
        {
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
        return (Mathf.Min(Roll.Basic(tempWeaponSkillValue + attributes.agility + modifier + recoil), weapon.Accuracy), defenseModifier);
    }

    public int DefendAgainstWeaponAttack(int modifier = 0)
    {
        return Roll.Basic(attributes.reaction + attributes.intuition + modifier);
    }

    public int DetermineDamageDealt(int netHits)
    {
        return EquippedItems.EquippedWeapon.Damage + netHits;
    }

    public int RollToResistDamage(int damage, int ap)
    {
        int armorVal = EquippedItems.EquippedArmor.ArmorRating + ap;
        if (damage > armorVal) return Roll.Basic(attributes.body + armorVal);
        return -1;
    }

    public DamagedState TakeDamage(int damage)
    {
        DamagedState prevState = CurrentDamagedState;
        DamageTaken += damage;
        if (CurrentDamagedState != prevState) GameLog.Instance.PostMessageToLog($"{alias} is {CurrentDamagedState}");
        return CurrentDamagedState;
    }

    public class CombatState
    {
        public bool IsTakingTurn { get; set; } = false;
        public int ActionCount { get; set; } = 0;

    }
    #endregion

    [System.Serializable]
    public class Equipped
    {
        public Weapon EquippedWeapon { get; private set; }
        public Armor EquippedArmor { get; private set; }

        public Equipped(Weapon weapon, Armor armor)
        {
            EquippedWeapon = weapon;
            EquippedArmor = armor;
        }

        public void Equip(Weapon newArmor) { EquippedWeapon = newArmor; }

        public void Equip(Armor newArmor) { EquippedArmor = newArmor; }
    }
}
