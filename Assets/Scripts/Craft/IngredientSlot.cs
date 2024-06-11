using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public int id;
    public Image icon;
    public TMP_Text txtQuantity;
    public Button button;
    private CraftData craftData;
    public bool canCraft = false;

    public CraftData CRData
    {
        get => craftData;
        set
        {
            craftData = value;
            if(craftData !=  null)
            {
                Set();
            }
            else
            {
                Clear();
            }
        }
    }
    private void Set()
    {
        int inventoryQuantity = CharacterManager.Instance.Player.inventory.GetItemQuantity(craftData.needs[id].needItem);
        icon.sprite = craftData.needs[id].needItem.icon;

        if (inventoryQuantity > craftData.needs[id].neadedQuantity)
        {
            button.interactable = true;
            canCraft = true;
        }
        else
        {
            button.interactable = false;
            canCraft = false;
        }
        txtQuantity.text = $"{inventoryQuantity} / {craftData.needs[id].neadedQuantity}";
        gameObject.SetActive(true);
        
    }
    private void Clear()
    {
        id = -1;
        icon.sprite = null;
        canCraft = false;
        button.interactable = false;
        txtQuantity.text = "";
        gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        if(CRData != null)
        {
            Set();
        }
        else
        {
            Clear();
        }
    }
}
