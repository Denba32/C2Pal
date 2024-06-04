using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PortionItem", menuName = "ItemData/PortionItemData", order = 0)]
public class PortionItemData : ConsumableItemData
{
    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    private void OnValidate()
    {
#if UNITY_EDITOR
        displayName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

