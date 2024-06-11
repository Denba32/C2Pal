using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ItemData/WeaponItemData", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    [Header("Attack Info")]
    public float attackRate;
    public float attackDistance;
    public float useStamina;

    [Header("Combat")]
    public float value;

    [Header("Resource Tool")]
    public Define.ResourceType resourceType;
}
