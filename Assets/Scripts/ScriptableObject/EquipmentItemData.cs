using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemData : ItemData
{
    public Define.EquipmentType equipmentType;
    public int requireLevel;
    public GameObject equipPrefab;

    [Header("Equip Offset")]
    public Vector3 posOffset;
    public Vector3 rotOffset;
}