using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOrb : MonoBehaviour
{
    public BossSO statSO;

    void Start()
    {
        Invoke("DestroyObject", 15f);
    }

    void Update()
    {
        Vector3 dir = (CharacterManager.Instance.Player.transform.position - transform.position).normalized;

        transform.position += dir * Time.deltaTime * 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.condition.uiconditions.health.Substract(statSO.damage);
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
