using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScratch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
            Debug.Log("�����⸦ ���߽��ϴ�!");
        }
    }

}
