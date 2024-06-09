using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class BossShadow : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    public float dashSpeed;
    private bool isDamaged;

    private IObjectPool<BossShadow> _ManagedPool;
    private Vector3 spawnPoint;    // 스폰 시점 (y는 무조건 고정, x, z는 알아서 설정하도록)
    private float maxSideDistance; // 소환할 때 좌우 시점
    private bool isTrueXFalseZ; 

    void Start()
    {
        RandomSpawn();
    }

    void OnEnable()
    {
        RandomSpawn();
        isDamaged = false;
    }

    void RandomSpawn()
    {
        float randomSide = Random.Range(-maxSideDistance, maxSideDistance);
        if (isTrueXFalseZ) // 참이면 X false면 z입니다.
        {
            transform.position = new Vector3(randomSide, spawnPoint.y, spawnPoint.z);
        }
        else
        {
            transform.position = new Vector3(spawnPoint.x, spawnPoint.y, randomSide);
        }

        agent.acceleration = 25f;
        agent.speed = 25f;
        animator.SetBool("Run", true);
        NavMesh.SamplePosition(transform.position + (Vector3.forward * 100f), out NavMeshHit hit, 100f, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isDamaged == false)
        {
            isDamaged = true;
            Debug.Log("그림자 습격에 맞았습니다1");
        }
    }

    void Update()
    {
        if (agent.remainingDistance < 0.2f)
        {
            DestroyShadow();
        }
    }

    // 페이즈 2를 위한 것
    public void InitObject(Vector3 _spawnPosition, float _maxDistance, bool _isTrueXFalseZ, float _dashSpeed)
    {
        spawnPoint = _spawnPosition;
        maxSideDistance = _maxDistance;
        isTrueXFalseZ = _isTrueXFalseZ;
        dashSpeed = _dashSpeed;
    }

    public void SetManagedPool(IObjectPool<BossShadow> pool)
    {
        _ManagedPool = pool;
    }

    public void DestroyShadow()
    {
        _ManagedPool.Release(this);
    }
}
