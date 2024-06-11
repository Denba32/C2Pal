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
            CharacterManager.Instance.Player.condition.uiconditions.health.Substract(statSO.damage);
        }
    }

}
