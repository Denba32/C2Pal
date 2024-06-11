using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PortionItem", menuName = "ItemData/PortionItemData", order = 0)]
public class PortionItemData : ConsumableItemData
{
    [Header("Portion")]
    public ItemDataConsumable[] consumables;

}