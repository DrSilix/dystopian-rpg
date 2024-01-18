using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Contact")]
public class ContactSO : ScriptableObject
{
    public string displayName;
    //TODO: public ContactType contactType;
    public int relation;
    public int favors;
    public int influence;

    public bool isKnown;

    public ShopInventorySO shopInventorySO;
    public float buyMultiplier;
    public float sellMultiplier;
    public int cashOnHand;
}
