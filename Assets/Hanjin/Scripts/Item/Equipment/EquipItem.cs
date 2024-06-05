using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipItem : ItemObject
{
    public new EquipmentItemData data
    {
        get => (EquipmentItemData)base.data;
        set => base.data = value;
    }
    public virtual void Equip()
    {

    }

    public virtual void UnEquip()
    {

    }

    public override string GetInteractPrompt()
    {
        if(data == null)
        {
            return base.GetInteractPrompt();
        }
        string str = $"{data.displayName}";
        return str;
    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}
