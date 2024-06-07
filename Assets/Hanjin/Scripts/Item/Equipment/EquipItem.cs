using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class EquipItem : ItemObject
{
    public ParentConstraint constraint;

    private ConstraintSource equipPos;
    public new EquipmentItemData data
    {
        get => (EquipmentItemData)base.data;
        set => base.data = value;
    }
    public virtual void Equip(Transform parent)
    {
        equipPos.sourceTransform = parent;
        equipPos.weight = 1;
        constraint.AddSource(equipPos);
        constraint.SetTranslationOffset(0, data.posOffset);
        constraint.SetRotationOffset(0, data.rotOffset);
        constraint.constraintActive = true;
    }

    public virtual void UnEquip()
    {
        if(constraint.sourceCount > 0)
        {
            constraint.RemoveSource(0);
        }
        constraint.constraintActive = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            OnHit(other);
        }


    }

    // 무언가에 충격을 줬을 때
    public virtual void OnHit(Collider target)
    {
        if (data.equipmentType == Define.EquipmentType.Armor)
            return;
    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}
