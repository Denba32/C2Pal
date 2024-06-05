using UnityEngine;

public class PortionItem : ConsumableItem
{
    public new PortionItemData data
    {
        get => (PortionItemData)base.data;
        set => base.data = value;
    }


    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} ���� �ݱ�");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }


}