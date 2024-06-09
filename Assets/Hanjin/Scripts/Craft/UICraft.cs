using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICraft : PopupUI
{
    private Dictionary<int, CraftData> craftDict;

    public UIInventory inventory;


    [Header("Receipe Slot")]
    public GameObject slot;
    public Transform slotRoot;
    public CraftSlot[] slots;

    public GameObject receipePanel;

    [Header("Receipe Slot")]
    public GameObject craftPanel;
    public IngredientSlot[] ingredientSlots;

    public Button btnCraft;

    private CraftData selectedData;
    public CraftData SelectedData
    {
        get
        {
            return selectedData;
        }
        set
        {
            selectedData = value;
            onSelected?.Invoke(selectedData);
        }
    }

    public Image targetIcon;
    public TMP_Text txt_targetQuantity;

    public event Action<CraftData> onSelected;

    public override void Init()
    {
        base.Init();

        craftDict = GameManager.Instance.Data.GetCraftData();
        slots = new CraftSlot[craftDict.Count];

        CraftData[] data = craftDict.Values.ToArray();
        
        for(int i = 0; i < data.Length; i++)
        {
            slots[i] = Instantiate(slot, slotRoot).GetComponent<CraftSlot>();
            slots[i].craftData = data[i];
            slots[i].uiCraft = this;
            slots[i].UpdateUI();

        }
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        craftPanel.SetActive(false);

        onSelected += SetReceipeIngredient;
        btnCraft.onClick.AddListener(Craft);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onSelected -= SetReceipeIngredient;
        btnCraft.onClick.RemoveListener(Craft);

    }

    private void SetReceipeIngredient(CraftData craftData)
    {
        craftPanel.SetActive(true);
        for(int i = 0; i < ingredientSlots.Length; i++)
        {
            ingredientSlots[i].CRData = null;
        }

        if(craftData != null)
        {
            targetIcon.sprite = craftData.craftTarget.icon;
            txt_targetQuantity.text = $"{craftData.craftQuantity}";
        }
        bool result = true;
        for(int i = 0; i < craftData.needs.Length; i++)
        {
            ingredientSlots[i].id = i;
            ingredientSlots[i].CRData = craftData;

            if (!ingredientSlots[i].canCraft)
            {
                result = false;
            }
        }

        if(!result)
        {
            btnCraft.interactable = false;
        }
        else
        {
            btnCraft.interactable = true;

        }
    }
    
    private void Craft()
    {
        for(int i = 0; i < SelectedData.needs.Length; i++)
        {
            if(CharacterManager.Instance.Player.inventory.UseItem(SelectedData.needs[i].needItem, SelectedData.needs[i].neadedQuantity))
            {

            }
            else
            {
                break;
            }
        }

        CharacterManager.Instance.Player.inventory.AddItem(SelectedData.craftTarget, SelectedData.craftQuantity);

        for(int i =0; i < ingredientSlots.Length; i++)
        {
            ingredientSlots[i].UpdateUI();
        }
    }

    public void CloseUI()
    {
        UIManager.Instance.ShowCraft();
    }
}
