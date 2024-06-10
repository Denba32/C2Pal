
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class UIInventory : PopupUI
{
    public int Gold { get; set; }

    [Header("Slot")]
    public ItemSlot[] slots;

    public GameObject slotPrefab;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    // 플레이어가 아이템을 떨굴 때 위치
    public Transform dropPosition;
        
    public UIDescription descriptionPanel;

    [Header("Select Item")]

    public GameObject selectedPanel;
    public GameObject selectedBlocker; 
    public RectTransform selectedContent;

    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    // 플레이어의 정보를 미리 참조
    private PlayerController controller;
    // 플레이어의 상태를 미리 참조
    private PlayerCondition condition;
    private PlayerInventory inventory;

    // 인벤에서 선택한 아이템 캐싱
    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curPrimaryWeaponIndex;
    int curSecondaryWeaponIndex;
    int curArmorIndex;



    private void OnDisable()
    {
        descriptionPanel.gameObject.SetActive(false);
    }


    public override void Init()
    {
        base.Init();

        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        inventory = CharacterManager.Instance.Player.inventory;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        slots = new ItemSlot[inventory.maxCapacity];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = CreateSlot(slotPanel);
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].description = descriptionPanel;
        }


        UpdateUI();

        //if (CharacterManager.Instance.Player.inventory.PlayerInven != null)
        //{
        //    if (CharacterManager.Instance.Player.inventory.PlayerInven.Count > 0)
        //    {
        //        if (slots != null)
        //        {
        //            var data = CharacterManager.Instance.Player.inventory.PlayerInven.ToArray();
        //            for (int i = 0; i < data.Length; i++)
        //            {
        //                UpdateUI(data[i].Value);
        //            }
        //        }
        //    }
        //}

        descriptionPanel.gameObject.SetActive(false);
        selectedBlocker.SetActive(false);
        selectedPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }



    private ItemSlot CreateSlot(Transform parent)
    {
        ItemSlot slot = null;
        Instantiate(slotPrefab, parent).TryGetComponent(out slot);
        return slot;
    }

    public void CloseInventory()
    {
        UIManager.Instance.ShowInventory();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    // 인벤토리에 주운 아이템 넣기
    public void UpdateUI(Item data)
    {
        if (data.quantity <= 0)
        {
            RemoveSelectedItem(data.slotId);
            UpdateUI();

            return;
        }
        ItemSlot slot = slots[data.slotId];
        slot.item = data.itemData;
        slot.quantity = data.quantity;

        Debug.Log("UI 슬롯에 저장된 Quantity : " + slot.quantity);

        UpdateUI();

    }

    #region 정렬 기능
    private void Sort(Define.ItemType type)
    {
        foreach (ItemSlot slot in slots)
        {
            if (type == Define.ItemType.None)
            {
                slot.gameObject.SetActive(true);
                continue;
            }
            if (slot.item != null)
            {
                if (slot.item.itemType != type)
                {
                    slot.gameObject.SetActive(false);
                }
                else
                {
                    slot.gameObject.SetActive(true);
                }
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void SortAll() => Sort(Define.ItemType.None);
    public void SortWeapon() => Sort(Define.ItemType.Weapon);
    public void SortResource() => Sort(Define.ItemType.Resource);
    public void SortConsumable() => Sort(Define.ItemType.Consumable);

    #endregion

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedBlocker.SetActive(true);
        selectedPanel.gameObject.SetActive(true);

        useButton.SetActive(selectedItem.itemType == Define.ItemType.Consumable);
        equipButton.SetActive((selectedItem.itemType == Define.ItemType.Weapon || selectedItem.itemType == Define.ItemType.Armor) && !slots[index].equipped);
        unequipButton.SetActive((selectedItem.itemType == Define.ItemType.Weapon || selectedItem.itemType == Define.ItemType.Armor) && slots[index].equipped);
        dropButton.SetActive(true);
    }
    void RemoveSelectedItem()
    {
        CharacterManager.Instance.Player.inventory.RemoveItem(selectedItemIndex);
    }
    void RemoveSelectedItem(int index)
    {
        slots[index].quantity--;
        Debug.Log(slots[index].quantity + "슬롯의 정보를 확인해보자");
        if (slots[index].quantity <= 0)
        {
            selectedItem = null;
            slots[index].item = null;
            selectedItemIndex = -1;
        }
        selectedPanel.SetActive(false);
        selectedBlocker.SetActive(false);
    }

    // 아이템 사용
    public void OnUseButton()
    {
        ConsumableItemData consumable = selectedItem as ConsumableItemData;

        if (consumable != null)
        {
            if (consumable.consumableItemType == Define.ConsumableItemType.Portion)
            {
                PortionItemData portion = consumable as PortionItemData;

                if (portion != null)
                {
                    for (int i = 0; i < portion.consumables.Length; i++)
                    {
                        switch (portion.consumables[i].type)
                        {
                            case Define.ConsumableValueType.Health:
                                if (condition.Heal(portion.consumables[i].value)) { }
                                else
                                {
                                    selectedPanel.SetActive(false);
                                    selectedBlocker.SetActive(false);

                                    return;
                                }
                                break;
                            case Define.ConsumableValueType.Hunger:
                                condition.Eat(portion.consumables[i].value);
                                break;
                            case Define.ConsumableValueType.Stamina:
                                condition.RestoreStamina(portion.consumables[i].value);
                                break;
                        }
                    }


                }
                RemoveSelectedItem();
            }
        }
    }
    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void ThrowItem(ItemData data)
    {
        EquipmentItemData tmp = data as EquipmentItemData;
        
        if (tmp != null)
        {
            if (tmp.equipmentType == Define.EquipmentType.PrimaryWeapon || tmp.equipmentType == Define.EquipmentType.ResourceTool)
            {
                UnEquip(curPrimaryWeaponIndex);
            }
            else if (tmp.equipmentType == Define.EquipmentType.Armor)
            {
                UnEquip(curPrimaryWeaponIndex);

            }
        }
        
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void OnEquipButton()
    {
        // 해당 아이템이 정말 Equipment인지 확인
        EquipmentItemData equipItem = selectedItem as EquipmentItemData;
        if (equipItem == null)
        {
            UpdateUI();
            return;
        }

        // 타입별로 무기 장착
        if (equipItem.equipmentType == Define.EquipmentType.PrimaryWeapon || equipItem.equipmentType == Define.EquipmentType.ResourceTool)
        {
            // 장착중인 아이템이 있을 경우 
            if (slots[curPrimaryWeaponIndex].equipped)
            {
                // 해당 아이템을 해제
                UnEquip(curPrimaryWeaponIndex);
            }

            CharacterManager.Instance.Player.primaryWeapon.Equip(equipItem);

            curPrimaryWeaponIndex = selectedItemIndex;
        }
        else if (equipItem.equipmentType == Define.EquipmentType.SecondaryWeapon)
        {
            // 장착중인 아이템이 있을 경우 
            if (slots[curSecondaryWeaponIndex].equipped)
            {
                // 해당 아이템을 해제
                UnEquip(curSecondaryWeaponIndex);
            }

            CharacterManager.Instance.Player.secondaryWeapon.Equip(equipItem);

            curSecondaryWeaponIndex = selectedItemIndex;
        }
        else if (equipItem.equipmentType == Define.EquipmentType.Armor)
        {
            // 장착중인 아이템이 있을 경우 
            if (slots[curArmorIndex].equipped)
            {
                // 해당 아이템을 해제
                UnEquip(curArmorIndex);
            }
            curArmorIndex = selectedItemIndex;
        }

        slots[selectedItemIndex].equipped = true;

        UpdateUI();

        selectedPanel.SetActive(false);
        selectedBlocker.SetActive(false);

    }
    void UnEquip(int index)
    {
        slots[index].equipped = false;
        EquipmentItemData tmp = slots[index].item as EquipmentItemData;
        if (tmp == null)
            return;

        if (tmp.equipmentType == Define.EquipmentType.PrimaryWeapon || tmp.equipmentType == Define.EquipmentType.ResourceTool)
        {
            CharacterManager.Instance.Player.primaryWeapon.UnEquip();
        }
        else if (tmp.equipmentType == Define.EquipmentType.SecondaryWeapon)
        {
            CharacterManager.Instance.Player.secondaryWeapon.UnEquip();
        }
        UpdateUI();
        selectedPanel.SetActive(false);
        selectedBlocker.SetActive(false);

    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}