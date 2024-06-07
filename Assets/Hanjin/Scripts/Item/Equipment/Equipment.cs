
using UnityEngine;
using UnityEngine.Animations;

public class Equipment : MonoBehaviour
{
    public EquipItem curEquip;

    public Transform equipPos;

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
        curEquip = Instantiate(data.equipPrefab, transform).GetComponent<EquipItem>();

        curEquip.name = data.equipPrefab.name;
        curEquip.Equip(equipPos);
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            curEquip.UnEquip();
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

}
