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
}