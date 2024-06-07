
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class UIInventory : MonoBehaviour
{
    public int Gold { get; set; }

    [Header("ItemInfo")]
    private Dictionary<Define.ItemType, ItemSlot[]> inventoryDict = new Dictionary<Define.ItemType, ItemSlot[]>();

    [Header("Slot")]
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    // �÷��̾ �������� ���� �� ��ġ
    public Transform dropPosition;
        
    public GameObject descriptionPanel;

    [Header("Select Item")]

    public GameObject selectedPanel;
    public GameObject selectedBlocker;
    public RectTransform selectedContent;

    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    // �÷��̾��� ������ �̸� ����
    private PlayerController controller;
    // �÷��̾��� ���¸� �̸� ����
    private PlayerCondition condition;

    // �κ����� ������ ������ ĳ��
    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curPrimaryWeaponIndex;
    int curSecondaryWeaponIndex;
    int curArmorIndex;

    private void Start()
    {
        Init();

        CharacterManager.Instance.Player.onAddItem += AddItem;

    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Init()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        //dropPosition = CharacterManager.Instance.Player.dropPosition;
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        inventoryDict.Add(Define.ItemType.None, slots);

        UpdateUI();

        descriptionPanel.SetActive(false);
        selectedBlocker.SetActive(false);
        selectedPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    // �κ��丮 â�� �����ִ��� ���� �Ǵ�
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }


    ItemSlot GetItemStack(ConsumableItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetItemStack(ResourceItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    
    /// <summary>
    /// 
    /// [���]
    /// 
    /// slot�� ���� slot�� ������ ������ ���� ��
    /// slot�� Set
    /// slot�� ������ ������ ���� ��
    /// slot�� Clear
    /// </summary>
    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
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

    // �κ��丮�� �ֿ� ������ �ֱ�
    void AddItem()
    {
        // �÷��̾ ��ȣ�ۿ��� ������ ������
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.itemType == Define.ItemType.Consumable)
        {
            ConsumableItemData csData = data as ConsumableItemData;
            if (csData != null)
            {
                ItemSlot slot = GetItemStack(csData);
                if (slot != null)
                {
                    slot.quantity++;
                    UpdateUI();
                    CharacterManager.Instance.Player.itemData = null;
                    return;
                }
            }
        }
        else if (data.itemType == Define.ItemType.Resource)
        {
            ResourceItemData rsData = data as ResourceItemData;
            if (rsData != null)
            {
                ItemSlot slot = GetItemStack(rsData);
                if (slot != null)
                {
                    slot.quantity++;
                    UpdateUI();
                    CharacterManager.Instance.Player.itemData = null;
                    return;
                }
            }
        }

        // �� ������ ã�Ƽ� ��ȯ
        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);

        CharacterManager.Instance.Player.itemData = null;
        inventoryDict.TryAdd(Define.ItemType.None, slots);
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedBlocker.SetActive(true);
        selectedPanel.gameObject.SetActive(true);

        useButton.SetActive(selectedItem.itemType == Define.ItemType.Consumable);
        equipButton.SetActive((selectedItem.itemType == Define.ItemType.Weapon || selectedItem.itemType == Define.ItemType.Armor)&& !slots[index].equipped);
        unequipButton.SetActive((selectedItem.itemType == Define.ItemType.Weapon || selectedItem.itemType == Define.ItemType.Armor) && slots[index].equipped);
        dropButton.SetActive(true);
    }

    #region ���� ���
    private void Sort(Define.ItemType type)
    {
        foreach (ItemSlot slot in slots)
        {
            if(type == Define.ItemType.None)
            {
                slot.gameObject.SetActive(true);
                continue;
            }
            if(slot.item != null)
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

    #region ������ ��� �Լ�


    // ������ ���
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
    
    public void OnEquipButton()
    {
        // �ش� �������� ���� Equipment���� Ȯ��
        EquipmentItemData equipItem = selectedItem as EquipmentItemData;
        if (equipItem == null)
        {
            UpdateUI();
            return;
        }

        // Ÿ�Ժ��� ���� ����
        if (equipItem.equipmentType == Define.EquipmentType.PrimaryWeapon || equipItem.equipmentType == Define.EquipmentType.ResourceTool)
        {
            // �������� �������� ���� ��� 
            if (slots[curPrimaryWeaponIndex].equipped)
            {
                // �ش� �������� ����
                UnEquip(curPrimaryWeaponIndex);
            }

            CharacterManager.Instance.Player.primaryWeapon.Equip(equipItem);

            curPrimaryWeaponIndex = selectedItemIndex;
        }
        else if (equipItem.equipmentType == Define.EquipmentType.SecondaryWeapon)
        {
            // �������� �������� ���� ��� 
            if (slots[curSecondaryWeaponIndex].equipped)
            {
                // �ش� �������� ����
                UnEquip(curSecondaryWeaponIndex);
            }

            CharacterManager.Instance.Player.secondaryWeapon.Equip(equipItem);

            curSecondaryWeaponIndex = selectedItemIndex;
        }
        else if(equipItem.equipmentType == Define.EquipmentType.Armor)
        {
            // �������� �������� ���� ��� 
            if (slots[curArmorIndex].equipped)
            {
                // �ش� �������� ����
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
        if(tmp == null)
            return;

        if (tmp.equipmentType == Define.EquipmentType.PrimaryWeapon || tmp.equipmentType == Define.EquipmentType.ResourceTool)
        {
            CharacterManager.Instance.Player.primaryWeapon.UnEquip();
        }
        else if(tmp.equipmentType == Define.EquipmentType.SecondaryWeapon)
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

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void ThrowItem(ItemData data)
    {
        EquipmentItemData tmp = data as EquipmentItemData;
        if(tmp != null)
        {
            if(tmp.equipmentType == Define.EquipmentType.PrimaryWeapon)
            {
                UnEquip(curPrimaryWeaponIndex);
            }
            else if(tmp.equipmentType == Define.EquipmentType.Armor)
            {
                UnEquip(curPrimaryWeaponIndex);

            }
        }
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    void RemoveSelectedItem()
    {

        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
        }

        UpdateUI();

        selectedPanel.SetActive(false);
        selectedBlocker.SetActive(false);
    }

    #endregion
}