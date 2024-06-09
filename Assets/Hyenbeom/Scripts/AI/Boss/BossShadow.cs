using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class BossShadow : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    public float dashSpeed;
    private bool isDamaged;

    private IObjectPool<BossShadow> _ManagedPool;
    private Vector3 spawnPoint;    // ���� ���� (y�� ������ ����, x, z�� �˾Ƽ� �����ϵ���)
    private float maxSideDistance; // ��ȯ�� �� �¿� ����
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
        if (isTrueXFalseZ) // ���̸� X false�� z�Դϴ�.
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
            Debug.Log("�׸��� ���ݿ� �¾ҽ��ϴ�1");
        }
    }

    void Update()
    {
        if (agent.remainingDistance < 0.2f)
        {
            DestroyShadow();
        }
    }

    // ������ 2�� ���� ��
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
