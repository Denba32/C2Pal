using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;



[Serializable]
public class PlayerInventory
{
    private Dictionary<int, Item> playerInven;

    public Dictionary<int, Item> PlayerInven
    {
        get
        {
            if (playerInven == null)
            {
                // TODO : 데이터 가져오기

                playerInven = new Dictionary<int, Item>();
            }
            return playerInven;
        }
    }

    private UIInventory uiInventory;
    public UIInventory UIInven
    {
        get
        {
            if(uiInventory == null)
            {
                uiInventory = UIManager.Instance.Inventory;
            }
            return uiInventory;
        }
    }

    [Header("Inventory Info")]
    public int maxCapacity;

    private int capacity;
    public int CurrentCapacity
    {
        get => capacity;
        set
        {
            capacity = value;
        }
    }

    public int gold;

    public GameObject uiPrefab;

    // 아이템 단일 추가
    public void AddItem(ItemData itemData)
    {
        if(itemData != null)
        {
            Item item = null;


            // 소비템이거나 자원템일 경우
            if (itemData.itemType == Define.ItemType.Consumable || itemData.itemType == Define.ItemType.Resource)
            {
                item = GetStackItem(itemData);
                if (item != null)
                {
                    item.quantity++;
                    Debug.Log(item.itemData.displayName + " 개수 : " +  item.quantity);
                    PlayerInven.TryAdd(item.slotId, item);
                    SoundManager.Instance.Play("PickupSound", Define.SoundType.Effect);

                    UIInven.UpdateUI(item);
                    return;

                }

            }
            // 새로운 아이템을 추가하려고 할 시 Full인지 확인
            if (maxCapacity <= CurrentCapacity)
            {
                UIManager.Instance.MainUI.SetAlert("가방 공간이 부족합니다.");
                CharacterManager.Instance.Player.ITData = null;
                ResourceManager.Instance.Instantiate(itemData.dropPrefab, CharacterManager.Instance.Player.dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360));
                return;
            }
            item = new Item();

            int id = GetEmptyItemSlot();

            item.hashId = item.GetHashCode();
            item.slotId = id;
            item.quantity++;
            item.itemData = itemData;

            PlayerInven.TryAdd(item.slotId , item);
            CurrentCapacity++;

            SoundManager.Instance.Play("PickupSound", Define.SoundType.Effect);
            UIInven.UpdateUI(item);
            CharacterManager.Instance.Player.ITData = null;
            
        }
        // TODO UIInventory에게 알리기
    }

    // 아이템 복수 추가
    public void AddItem(ItemData itemData, int quantity)
    {
        if (itemData != null)
        {
            Item item = null;
            int otherQuantity = 0;
            if (itemData.itemType == Define.ItemType.Consumable || itemData.itemType == Define.ItemType.Resource)
            {
                item = GetStackItem(itemData);
                if (item != null)
                {
                    // 스택 가능 수를 넘을 경우
                    if(item.itemData.maxAmount < (item.quantity + quantity))
                    {
                        // 넣을 수 있는 만큼을 넣은 후
                        int lastQuantity = item.itemData.maxAmount - item.quantity;
                        item.quantity += lastQuantity;

                        int opticalQuantity = quantity - lastQuantity;
                        PlayerInven.TryAdd(item.slotId, item);
                        SoundManager.Instance.Play("PickupSound", Define.SoundType.Effect);

                        UIInven.UpdateUI(item);

                        // 다시 새로운 누적 공간을 찾음
                        item = GetStackItem(itemData);

                        // 만약 누적 공간이 있다면 추가로 누적
                        if (item != null)
                        {
                            item.quantity += opticalQuantity;
                            PlayerInven.TryAdd(item.slotId, item);
                            UIInven.UpdateUI(item);
                            return;
                        }
                        otherQuantity = opticalQuantity;

                    }
                    else
                    {
                        item.quantity += quantity;
                        Debug.Log(item.itemData.displayName + " 개수 : " + item.quantity);
                        PlayerInven.TryAdd(item.slotId, item);
                        SoundManager.Instance.Play("PickupSound", Define.SoundType.Effect);

                        UIInven.UpdateUI(item);

                        return;
                    }
                }

            }
            if (maxCapacity <= CurrentCapacity)
            {
                UIManager.Instance.MainUI.SetAlert("가방 공간이 부족합니다.");
                CharacterManager.Instance.Player.ITData = null;

                ResourceManager.Instance.Instantiate(itemData.dropPrefab, CharacterManager.Instance.Player.dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360));
                // 경고 알림이 떠야함
                return;
            }
            item = new Item();

            int id = GetEmptyItemSlot();

            item.hashId = item.GetHashCode();
            item.slotId = id;
            if (otherQuantity > 0)
            {
                item.quantity += otherQuantity;
            }
            else
                item.quantity = quantity;
            item.quantity += quantity;
            item.itemData = itemData;

            PlayerInven.TryAdd(item.slotId, item);
            CurrentCapacity++;
            SoundManager.Instance.Play("PickupSound", Define.SoundType.Effect);

            UIInven.UpdateUI(item);
            CharacterManager.Instance.Player.ITData = null;

        }
        // TODO UIInventory에게 알리기
    }

    public void UpdateItem(Item item)
    {
        if(item != null)
        {
            // 다쓴 경우
            if(item.quantity <= 0)
            {
                RemoveItem(item.slotId);
            }
            else if(item.quantity > 0)
            {
                PlayerInven.Add(item.slotId, item);
            }
        }
    }

    
    private int GetEmptyItemSlot()
    {
        int slotId = -1;
        for(int i = 0; i < maxCapacity;i++)
        {
            if (PlayerInven.ContainsKey(i))
            {
                continue;
            }
            else
            {
                slotId = i;
                break;
            }
        }

        return slotId;
    }

    // StackItem 찾기
    private Item GetStackItem(ItemData data)
    {
        if(data != null)
        {
            var filteredData = PlayerInven.Where(x => x.Value.itemData.id == data.id && x.Value.itemData.canStack && x.Value.quantity < x.Value.itemData.maxAmount).Select(x => x.Value).OrderBy(x => x.slotId).ToArray();

            if(filteredData != null)
            {
                if(filteredData.Length > 0)
                {
                    return filteredData[0];
                }

                return null;
            }
        }
        return null;
    }

    public int GetItemQuantity(ItemData data)
    {
        if(data != null)
        {
            var filteredData = PlayerInven.Where(x => x.Value.itemData.id == data.id && x.Value.itemData.canStack && x.Value.quantity < x.Value.itemData.maxAmount).Select(x => x.Value).ToArray();

            return filteredData.Sum(x => x.quantity);
        }

        return -1;
    }

    public bool UseItem(ItemData data, int quantity)
    {
        if(data != null)
        {
            var filter = GetStackItem(data);

            if(filter.quantity == quantity)
            {
                filter.quantity -= quantity;
                // UpdateItem(filter);
                return true;
            }
            else if(filter.quantity > quantity)
            {
                filter.quantity -= quantity;
                // UpdateItem(filter);
                return true;
            }
        }
        return false;

    }

    public void RemoveItem(int index)
    {
        if(index >= 0)
        {
            if(PlayerInven.ContainsKey(index))
            {
                if (PlayerInven[index].quantity >= 1)
                {
                    PlayerInven[index].quantity--;
                    uiInventory.UpdateUI(PlayerInven[index]);

                    if (PlayerInven[index].quantity <= 0)
                    {
                        PlayerInven.Remove(index);
                        if(CurrentCapacity > 0)
                        {
                            CurrentCapacity--;
                        }
                    }
                }

            }
        }
    }

    public void Equip(int index)
    {
        if (!PlayerInven[index].isEquipped)
            PlayerInven[index].isEquipped = true;

    }

    public void UnEquip(int index)
    {
        if(PlayerInven[index].isEquipped)
            PlayerInven[index].isEquipped = false;
    }

    public void SortItem()
    {

    }
    public void CheckItems()
    {
        Debug.Log($"현재 수용중인 아이템 수량{CurrentCapacity}");
        for(int i = 0; i < playerInven.Count; i++)
        {
            Debug.Log($"{i}번째 슬롯의 아이템 : {playerInven[i].itemData.displayName} 남은 개수 : {playerInven[i].quantity}");
        }
    }

    public bool IsFull()
    {
        if(CurrentCapacity >= maxCapacity)
        {
            return true;
        }
        return false;
    }
}