
using UnityEngine;

public class PrimaryWeapon : EquipItem
{
    public new WeaponItemData data
    {
        get => (WeaponItemData)base.data; 
        set => base.data = value;
    }

    private bool attacking;

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} 무기 줍기");
        CharacterManager.Instance.Player.ITData = data;
        CharacterManager.Instance.Player.onAddItem.Invoke(data);
        Destroy(gameObject);
    }

    public override void OnHit(Collider target)
    {
        base.OnHit(target);

        if(target.TryGetComponent(out ResourceObject resource))
        {
            if(data.equipmentType == Define.EquipmentType.PrimaryWeapon)
            {
                resource.Gather(target.ClosestPointOnBounds(transform.position), 1);
            }
            else if(data.equipmentType == Define.EquipmentType.ResourceTool)
            {
                if(data.resourceType == resource.resourceType)
                    resource.Gather(target.ClosestPointOnBounds(transform.position), (int)data.value);
                else
                    resource.Gather(target.ClosestPointOnBounds(transform.position), (int)(data.value * 0.5f));

            }
            return;
        }
        if(target.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(data.value);
            damagable.Damage(data.value * 0.5f);
            return;
        }
    }

}
