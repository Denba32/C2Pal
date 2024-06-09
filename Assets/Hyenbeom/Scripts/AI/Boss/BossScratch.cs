using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScratch : MonoBehaviour
{

    public BossSO statSO;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().Damage(statSO.damage);
        }
    }

}
