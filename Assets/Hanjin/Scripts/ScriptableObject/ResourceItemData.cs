using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItem", menuName = "ItemData/ResourceItemData", order = 3)]
public class ResourceItemData : ItemData
{
    [Header("Stacking")]
    public bool canStack;
    public int maxAmount;

    public AudioClip dropSound;
}