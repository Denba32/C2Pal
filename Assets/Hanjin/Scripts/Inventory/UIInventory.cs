using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class UIInventory : MonoBehaviour
{
    [Header("Slot")]
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    // �÷��̾ �������� ���� �� ��ġ
    public Transform dropPosition;

    [Header("Select Item")]
    public Image selectedItemIcon;
    public TMP_Text selectedItemName;
    public TMP_Text selectedItemDescription;
    public TMP_Text selectedEffectValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    // �÷��̾��� ������ �̸� ����
    private PlayerController controller;
    //// �÷��̾��� ���¸� �̸� ����
    //private PlayerCondition condition;

    // �κ����� ������ ������ ĳ��
    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;

    private void Start()
    {
        Init();

        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        // ClearSelectedItemWindow();

        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        CharacterManager.Instance.Player.onAddItem += AddItem;
        // ClearSelectedItemWindow();
    }
    private void OnDisable()
    {
        CharacterManager.Instance.Player.onAddItem -= AddItem;

    }

    private void Init()
    {
        controller = CharacterManager.Instance.Player.controller;
        //condition = CharacterManager.Instance.Player.condition;
        //dropPosition = CharacterManager.Instance.Player.dropPosition;
    }
    private void Update()
    {
        Debug.Log(EventSystem.current.IsPointerOverGameObject() + "PointerOver");

        // ��� �ִ� �������� ����
        Debug.Log(EventSystem.current.currentSelectedGameObject + "CurrentSelected");

        Debug.Log(EventSystem.current.firstSelectedGameObject + "FirstSelected");
        
    }


    // �κ��丮�� ������ ���� ���� �ʱ�ȭ ����
    void ClearSelectedItemWindow()
    {
        selectedItemIcon.sprite = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedEffectValue.text = string.Empty;

        selectedItemIcon.gameObject.SetActive(false);
        useButton.SetActive(false);
        equipButton.SetActive(false);   
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }



    // �κ��丮 â�� �����ִ��� ���� �Ǵ�
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    
    // �κ��丮�� �ֿ� ������ �ֱ�
    void AddItem()
    {
        // �÷��̾ ��ȣ�ۿ��� ������ ������
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.itemType == ItemType.Consumable)
        {
            ConsumableItemData csData = data as ConsumableItemData;
            if(csData != null)
            {
                ItemSlot slot = GetItemStack(csData);
                if(slot != null)
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


        /*
        if(data.canStack)
        {
            ItemSlot slot = GetItemStack(data);

            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            // UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }
        */
        // ������â�� ���� ���
        // �������� ��
        // TODO : �Ա� ����, �κ��� �뷮�� ������� üũ
        // ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
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



    /*

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
        {
            ClearSelectedItemWindow();
            return;
        }
        selectedItemIcon.gameObject.SetActive(true);
        selectedItemIcon.sprite = slots[index].item.icon;
        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedEffectValue.text = string.Empty;

        // �Һ����� ���
        for(int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedEffectValue.text += $"{selectedItem.consumables[i].type.ToString()} : {selectedItem.consumables[i].value.ToString()}\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    // ������ ���
    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch(selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;

                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            selectedItemIcon.sprite = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        Debug.Log(selectedItem);
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
    // ������ Swap ����

    // ������ ��ġ ���� ����
    */
}