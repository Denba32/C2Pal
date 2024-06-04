using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum ItemType
{
    Equipable,
    Consumable,
    Resource,
    Buff,
}

public enum ConsumableType
{
    Health,
    Hunger
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}
*/

public abstract class ItemData : ScriptableObject
{
    [Header("Info")]
    public int id;
    public string displayName;
    public string description;
    public ItemType itemType;
    public Sprite icon;
    public GameObject dropPrefab;

    /*
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header(("Equip"))]
    public GameObject equipPrefab;
    */
}
