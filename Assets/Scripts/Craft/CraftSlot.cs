using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftSlot : MonoBehaviour
{
    public UICraft uiCraft;

    public CraftData craftData;

    public TMP_Text txtReceipeName;

    public Button slotButton;

    private void OnEnable()
    {
        slotButton.onClick.AddListener(SelectSlot);
    }


    private void OnDisable()
    {
        slotButton.onClick.RemoveListener(SelectSlot);
    }

    private void SelectSlot()
    {
        uiCraft.SelectedData = craftData;
    }

    public void UpdateUI()
    {
        if(craftData != null)
        {
            txtReceipeName.text = craftData.receipeName;
        }

    }
}
