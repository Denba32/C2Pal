using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    [Header("Info")]
    public ItemData itemData;

    // 같은 아이템이지만 객체별로 구분짓기 위해서
    public int hashId;
    public int slotId;
    public int quantity;

    public bool isEquipped = false;
}