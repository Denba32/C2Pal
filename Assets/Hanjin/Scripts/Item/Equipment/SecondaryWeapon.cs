
using UnityEngine;

public class SecondaryWeapon : EquipItem
{
    public new WeaponItemData data
    {
        get => (WeaponItemData)base.data;
        set => base.data = value;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} 보조 무기 줍기");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }
}
