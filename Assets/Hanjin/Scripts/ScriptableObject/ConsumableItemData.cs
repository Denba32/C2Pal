

using System;
using UnityEngine;

public enum ConsumableType
{
    Health,
    Mana,
    Stamina,
    Hunger,
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

public class ConsumableItemData : ItemData
{
    [Header("Stacking")]
    public bool canStack;
    public int maxAmount;
}
