using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour
{
    public GameObject monsterPrefab;

    [Header("SpawnSettings")]
    public int maxSpawn;
    public float spawnDuration;

    // ���Ͱ� ������� ��ȯ�ؾ��� �� üũ��
    private List<GameObject> monsterList = new List<GameObject>();

    IEnumerator coroutineSpawn;

    // ��ġ ǥ�ÿ�
    public GameObject spawnMark;

    void Awake()
    {
        spawnMark.SetActive(false); // ��ġ ǥ�ÿ��� ��������մϴ�.
    }

    void Start()
    {
        MonsterSpawn();
    }

    void MonsterSpawn()
    {
        if (coroutineSpawn == null)
        {
            coroutineSpawn = MonsterSpawnCoroutine();
            StartCoroutine(coroutineSpawn);
        }
    }

    private IEnumerator MonsterSpawnCoroutine()
    {
        while (monsterList.Count < maxSpawn)
        {
            Vector3 spawnPoint = transform.position + UnityEngine.Random.onUnitSphere;
            spawnPoint = new Vector3(spawnPoint.x, transform.position.y, spawnPoint.z);
            GameObject monster = Instantiate(monsterPrefab, spawnPoint, Quaternion.identity);
            monsterList.Add(monster);
            yield return new WaitForSeconds(spawnDuration);
        }
        StopCoroutine(coroutineSpawn);
        coroutineSpawn = null;
    }

    public void MonsterDead(GameObject monster)
    {
        monsterList.Remove(monster);
        MonsterSpawn();
    }
}
