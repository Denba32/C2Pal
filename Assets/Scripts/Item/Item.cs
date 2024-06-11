using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    [Header("Info")]
    public ItemData itemData;

    // ���� ������������ ��ü���� �������� ���ؼ�
    public int hashId;
    public int slotId;
    public int quantity;

    public bool isEquipped = false;
}