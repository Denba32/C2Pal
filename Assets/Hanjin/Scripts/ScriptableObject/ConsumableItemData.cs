

using System;
using UnityEngine;

[Serializable]
public class ItemDataConsumable
{
    public Define.ConsumableValueType type;
    public float value;
}

public class ConsumableItemData : ItemData
{
    [Header("Consumable")]

    public Define.ConsumableItemType consumableItemType;

    public bool canStack;
    public int maxAmount;
}
