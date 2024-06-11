using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ConsumableItem : ItemObject
{
    public new ConsumableItemData data
    {
        get => (ConsumableItemData)base.data;
        set => base.data = value;
    }

    public int Amount { get; protected set; }

    public int MaxAmount => data.maxAmount;
    
    public bool IsMax => Amount >= MaxAmount;

    public bool IsEmpty => Amount <= 0;

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }
    public override string GetInteractPrompt()
    {
        if (data == null)
        {
            Debug.Log("해당 아이템 데이터가 존재하지 않습니다.");
            return base.GetInteractPrompt();
        }

        string str = $"{data.displayName}";
        return str;
    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}