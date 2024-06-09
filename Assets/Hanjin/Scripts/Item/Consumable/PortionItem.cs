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

        Debug.Log($"{data.displayName} �Ҹ�ǰ �ݱ�");
        CharacterManager.Instance.Player.ITData = data;
        CharacterManager.Instance.Player.onAddItem.Invoke(data);

        Destroy(gameObject);
    }

}