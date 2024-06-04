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

        Debug.Log($"{data.displayName} 포션 줍기");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}
