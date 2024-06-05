
using System;

public class Define
{
    public enum EquipmentType
    {
        Weapon,
        Armor,
    }

    [Flags]
    public enum EquipStatus
    {
        Nothing = 0,
        LeftHand = 1 << 0,
        RightHand = 1 << 1,
        Both = LeftHand | RightHand
    }

    public enum ItemType
    {
        None,
        Consumable,
        Weapon,
        Armor,
        Resource
    }

    public enum ConsumableItemType
    {
        Portion,
    }

    public enum ConsumableValueType
    {
        Health,
        Stamina,
        Hunger,
    }
}