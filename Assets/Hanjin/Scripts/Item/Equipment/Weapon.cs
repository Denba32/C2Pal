using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : EquipItem
{
    public new WeaponItemData data
    {
        get => (WeaponItemData)base.data; 
        set => base.data = value;
    }

    private bool attacking;



    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} 무기 줍기");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }

}
