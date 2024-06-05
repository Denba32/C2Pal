using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float Health = 10;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            Health -= 1;
            Debug.Log($"���� �����߽��ϴ�. ���� ü���� {Health} ��ŭ �������ϴ�.");
        }
           
    }
}
