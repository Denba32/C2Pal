using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionItem : ConsumableItem
{
    public new PortionItemData data
    {
        get => (PortionItemData)base.data;
        set => base.data = value;
    }

    public override string GetInteractPrompt()
    {
        if(data == null)
        {
            Debug.Log("해당 아이템 데이터가 존재하지 않습니다.");
            return base.GetInteractPrompt();
        }

        string str = "줍기";
        return str;
    }
    public override void OnInteract()
    {
        base.OnInteract();


    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}
