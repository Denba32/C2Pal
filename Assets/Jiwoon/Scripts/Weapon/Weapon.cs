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
            Debug.Log($"적을 공격했습니다. 적의 체력이 {Health} 만큼 남었습니다.");
        }
           
    }
}
