
using UnityEngine;


public class UIInventory : MonoBehaviour
{

    [Header("Slot")]
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    // 플레이어가 아이템을 떨굴 때 위치
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

    // 플레이어의 정보를 미리 참조
    private PlayerController controller;
    // 플레이어의 상태를 미리 참조
    private PlayerCondition condition;

    // 인벤에서 선택한 아이템 캐싱
    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curWeaponIndex;
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

    // 인벤토리 창이 열려있는지 여부 판단
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

    // 인벤토리에 주운 아이템 넣기
    void AddItem()
    {
        // 플레이어가 상호작요한 아이템 데이터
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

        ThrowItem(data);

        CharacterManager.Instance.Player.itemData = null;
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

    #region 아이템 사용 함수

    
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
        if (equipItem.equipmentType == Define.EquipmentType.Weapon)
        {
            // 장착중인 아이템이 있을 경우 
            if (slots[curWeaponIndex].equipped)
            {
                // 해당 아이템을 해제
                UnEquip(curWeaponIndex);
            }

            CharacterManager.Instance.Player.weapon.Equip(equipItem);

            curWeaponIndex = selectedItemIndex;

        }
        else if(equipItem.equipmentType == Define.EquipmentType.Armor)
        {
            // 장착중인 아이템이 있을 경우 
            if (slots[curArmorIndex].equipped)
            {
                // 해당 아이템을 해제
                UnEquip(curArmorIndex);
            }
            CharacterManager.Instance.Player.armor.Equip(equipItem);
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

        if(tmp.equipmentType == Define.EquipmentType.Weapon)
        {
            CharacterManager.Instance.Player.weapon.UnEquip();
        }
        else if(tmp.equipmentType == Define.EquipmentType.Armor)
        {
            CharacterManager.Instance.Player.armor.UnEquip();
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
            if(tmp.equipmentType == Define.EquipmentType.Weapon)
            {
                UnEquip(curWeaponIndex);
            }
            else if(tmp.equipmentType == Define.EquipmentType.Armor)
            {
                UnEquip(curWeaponIndex);

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