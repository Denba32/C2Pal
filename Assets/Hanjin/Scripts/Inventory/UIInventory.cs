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

    // 플레이어가 아이템을 떨굴 때 위치
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

    // 플레이어의 정보를 미리 참조
    private PlayerController controller;
    //// 플레이어의 상태를 미리 참조
    //private PlayerCondition condition;

    // 인벤에서 선택한 아이템 캐싱
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

        // 들고 있는 아이템의 정보
        Debug.Log(EventSystem.current.currentSelectedGameObject + "CurrentSelected");

        Debug.Log(EventSystem.current.firstSelectedGameObject + "FirstSelected");
        
    }


    // 인벤토리의 아이템 슬롯 제외 초기화 상태
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



    // 인벤토리 창이 열려있는지 여부 판단
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    
    // 인벤토리에 주운 아이템 넣기
    void AddItem()
    {
        // 플레이어가 상호작요한 아이템 데이터
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

        // 빈 슬롯을 찾아서 반환
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
        // 아이템창이 꽉찬 경우
        // 아이템을 뱉어냄
        // TODO : 먹기 전에, 인벤의 용량이 충분한지 체크
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
    /// [기능]
    /// 
    /// slot을 돌며 slot에 아이템 정보가 있을 시
    /// slot을 Set
    /// slot에 아이템 정보가 없을 시
    /// slot을 Clear
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

        // 소비템의 경우
        for(int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedEffectValue.text += $"{selectedItem.consumables[i].type.ToString()} : {selectedItem.consumables[i].value.ToString()}\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    // 아이템 사용
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
    // 아이템 Swap 구현

    // 아이템 위치 변경 구현
    */
}