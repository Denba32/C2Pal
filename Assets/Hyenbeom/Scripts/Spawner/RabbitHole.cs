using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [Header("SpawnSetting")]
    public float spawnDuration;
    public int maxSpawn;

    public float callTest;
    public float comeOutTest;

    private List<GameObject> rabbitList = new List<GameObject>();

    IEnumerator coroutineSpawn;
    IEnumerator coroutineComeOut;

    void Start()
    {
        // 낮과 밤이 됐을 때 이벤트에다가 넣어서 나오는 것과 Spawn을 조종할 것
        RabbitSpawn();
        StartCoroutine(RabbitCallTest());
        StartCoroutine(RabbitComeOutTest());
    }

    void RabbitSpawn()
    {
        if (coroutineSpawn == null)
        {
            coroutineSpawn = RabbitSpawnCoroutine();
            StartCoroutine(coroutineSpawn);
        }
    }

    IEnumerator RabbitCallTest()
    {
        yield return new WaitForSeconds(callTest);
        RabbitCall();
    }
    IEnumerator RabbitComeOutTest()
    {
        yield return new WaitForSeconds(comeOutTest);
        RabbitComeOut();
    }

    // 낮이 되었을 때 발동
    void RabbitComeOut()
    {
        if (coroutineComeOut == null)
        {
            coroutineComeOut = RabbitComeOutCoroutine();
            StartCoroutine(coroutineComeOut);
        }
    }

    IEnumerator RabbitSpawnCoroutine()
    {
        while (rabbitList.Count < maxSpawn)
        {
            Vector3 spawnPoint = transform.position + UnityEngine.Random.onUnitSphere;
            spawnPoint = new Vector3(spawnPoint.x , transform.position.y, spawnPoint.z);
            GameObject rabbit = Instantiate(prefab, spawnPoint, Quaternion.identity);
            rabbitList.Add(rabbit);
            rabbit.GetComponent<RabbitClass>().SetHome(this.gameObject);
            rabbit.GetComponent<RabbitClass>().DontWantHome();
            yield return new WaitForSeconds(spawnDuration);
        }
        StopCoroutine(coroutineSpawn);
        coroutineSpawn = null;
    }

    IEnumerator RabbitComeOutCoroutine()
    {
        foreach (GameObject rabbit in rabbitList)
        {
            if (rabbit.activeInHierarchy == true) { continue; }

            Vector3 spawnPoint = transform.position + UnityEngine.Random.onUnitSphere;
            spawnPoint = new Vector3(spawnPoint.x, transform.position.y, spawnPoint.z);
            rabbit.transform.position = spawnPoint;
            rabbit.SetActive(true);
            rabbit.GetComponent<RabbitClass>().DontWantHome();
            
            yield return new WaitForSeconds(5f);
        }
        StopCoroutine(coroutineComeOut);
        coroutineComeOut = null;
    }

    // 밤이 될 시 토끼들을 철수 시킬 것
    void RabbitCall()
    {
        foreach (GameObject rabbit in rabbitList)
        {
            rabbit.GetComponent<RabbitClass>().GoHome();
        }
    }

    public void RabbitGoHome(GameObject rabbit)
    {
        rabbit.SetActive(false);
    }

    public void RabbitDead(GameObject rabbit)
    {
        rabbitList.Remove(rabbit);
        RabbitSpawn();
    }
}
