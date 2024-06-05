using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EquipItem
{
    public new ArmorItemData data
    {
        get => (ArmorItemData)base.data;
        set => base.data = value;
    }
    public override void Equip()
    {
        base.Equip();
    }

    public override void UnEquip()
    {
        base.UnEquip();
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} 방어구 줍기");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }
}
