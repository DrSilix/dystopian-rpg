/*
 * Stores enums for all valid events
 * able to be queried for:
 * the script component type associated
 * TODO: able to return the group an event belongs to
 * TODO: a list of all events
 * TODO: a list of all events of a certain group
 * TODO: able to generate a random event from all events
 * TODO: able to generate a random event from a whitelist of groups
 * TODO: able to generate a random event from a blacklist of groups
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


public static class HEventType
{
    //TODO: should probably figure out a better way to do this enum but with a method thing
    public enum HType {
        //Pre-Heist Events
        Pre_InfoGathering,
        Pre_Planning,
        Pre_Navigating,
        Pre_AquirePackage,
        //Heist Events
        Hst_Recon,
        Hst_Sneaking,
        Hst_Masquerading,
        Hst_Hacking,
        Hst_Breaching,
        Hst_Trap,
        Hst_Spotted,
        Hst_HoldHostages,
        //Objective Events
        Obj_StealData,
        Obj_PlantData,
        Obj_StealItem,
        Obj_PlantItem,
        Obj_Kidnap,
        Obj_Extraction,
        Obj_Intimidate,
        Obj_DropOffPackage,
        Obj_DropOffPerson,
        Obj_SearchHostages,
        //Combat Events
        Cmbt_Combat,
        Cmbt_Chase,
        //Post-Heist Events
        Pst_Escape,
        Pst_ReturnHome
    }

    public static Type GetEventComponentType (HType type)
    {
        switch (type)
        {
            case HType.Pre_InfoGathering:
                break;
            case HType.Pre_Planning:
                break;
            case HType.Pre_Navigating:
                return Type.GetType("NavigationEvent");
            case HType.Pre_AquirePackage:
                break;
            case HType.Hst_Recon:
                break;
            case HType.Hst_Sneaking:
                return Type.GetType("SneakingEvent");
            case HType.Hst_Masquerading:
                return Type.GetType("MasqueradingEvent");
            case HType.Hst_Hacking:
                return Type.GetType("HackingEvent");
            case HType.Hst_Breaching:
                return Type.GetType("BreachingEvent");
            case HType.Hst_Trap:
                break;
            case HType.Hst_Spotted:
                break;
            case HType.Hst_HoldHostages:
                break;
            case HType.Obj_StealData:
                return Type.GetType("StealDataEvent");
            case HType.Obj_PlantData:
                break;
            case HType.Obj_StealItem:
                break;
            case HType.Obj_PlantItem:
                break;
            case HType.Obj_Kidnap:
                break;
            case HType.Obj_Extraction:
                break;
            case HType.Obj_Intimidate:
                break;
            case HType.Obj_DropOffPackage:
                break;
            case HType.Obj_DropOffPerson:
                break;
            case HType.Obj_SearchHostages:
                break;
            case HType.Cmbt_Combat:
                return Type.GetType("CombatEvent");
            case HType.Cmbt_Chase:
                break;
            case HType.Pst_Escape:
                break;
            case HType.Pst_ReturnHome:
                return Type.GetType("ReturnHomeEvent");
            default:
                break;
        }
        throw new System.Exception("Heist Type Invalid");
    }
}
