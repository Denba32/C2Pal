using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItem : ItemObject
{
    public new ResourceItemData data
    {
        get => (ResourceItemData)base.data;
        set => base.data = value;
    }

    public override string GetInteractPrompt()
    {
        if (data == null)
        {
            Debug.Log("�ش� ������ �����Ͱ� �������� �ʽ��ϴ�.");
            return base.GetInteractPrompt();
        }

        string str = $"{data.displayName}";
        return str;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} �ڿ� �ݱ�");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }

    public override InteractableType GetInteractableType() => InteractableType.Pickup;
}