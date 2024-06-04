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
            Debug.Log("�ش� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return base.GetInteractPrompt();
        }

        string str = "�ݱ�";
        return str;
    }
    public override void OnInteract()
    {
        base.OnInteract();


    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;

}
