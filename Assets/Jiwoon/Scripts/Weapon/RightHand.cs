using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    private float Health = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamagable[] damages = other.GetComponentsInParent<IDamagable>();

            for(int i = 0; i < damages.Length; i++)
            {
                if (damages[i] != null)
                {
                    damages[i].Damage(10);
                    Debug.Log("데미지 주기");
                }
            }
        }
           
    }
}
