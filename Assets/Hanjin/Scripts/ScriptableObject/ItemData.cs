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
    
    public Define.ItemType itemType;
    public Sprite icon;
    public GameObject dropPrefab;

    public int price;

    private void OnValidate()
    {
#if UNITY_EDITOR
        displayName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

}
