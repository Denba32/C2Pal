
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class Equipment : MonoBehaviour
{
    public EquipItem curEquip;

    public Transform equipPos;


    private PlayerController controller;

    private void Start()
    {
        controller = GetComponent<PlayerController>();

        GameManager.Instance.Data.Init();
    }

    public void Equip(EquipmentItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, transform).GetComponent<EquipItem>();

        curEquip.name = data.equipPrefab.name;
        curEquip.Equip(equipPos);

        PrimaryWeapon primaryWeapon = curEquip as PrimaryWeapon;

        if(primaryWeapon != null )
        {
            CharacterManager.Instance.Player.controller.meleeArea = primaryWeapon.meleeArea;
            CharacterManager.Instance.Player.controller.trailEffect = primaryWeapon.trailEffect;
        }
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            CharacterManager.Instance.Player.controller.meleeArea = null;
            CharacterManager.Instance.Player.controller.trailEffect = null;
            curEquip.UnEquip();

            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public bool IsEquipped => curEquip != null;
}