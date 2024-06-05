
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipItem curEquip;
    public Transform equipParent;

    private PlayerController controller;
    // private PlayerCondition condition;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        // condition = GetComponent<PlayerCondition>();
    }

    public void Equip(EquipmentItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<EquipItem>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

}
