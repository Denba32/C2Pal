using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOrb : MonoBehaviour
{
    public BossSO statS0;

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
            Debug.Log("구체에 맞았습니다!");
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
