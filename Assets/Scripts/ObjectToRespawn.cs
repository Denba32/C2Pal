using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAndRespawn : MonoBehaviour
{
    public GameObject objectToRespawn; 
    public float respawnTime = 5.0f;   

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyObjectAndRespawn();
        }
    }

    public void DestroyObjectAndRespawn()
    {
        Destroy(gameObject);

        StartCoroutine(RespawnAfterTime(respawnTime));
    }

    IEnumerator RespawnAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Instantiate(objectToRespawn, initialPosition, initialRotation);
    }
}