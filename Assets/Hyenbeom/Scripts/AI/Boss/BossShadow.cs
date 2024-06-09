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
    public float damage;
    private bool isDamaged;

    private IObjectPool<BossShadow> _ManagedPool;
    private Vector3 spawnPoint;    // ���� ���� (y�� ������ ����, x, z�� �˾Ƽ� �����ϵ���)
    private float maxSideDistance; // ��ȯ�� �� �¿� ����
    private bool isTrueXFalseZ;

    public AngleState angleState = AngleState.forward;

    private NavMeshHit hit;

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
        if (isTrueXFalseZ) // ���̸� x side false�� z side�Դϴ�.
        {
            transform.localPosition = new Vector3(randomSide, spawnPoint.y, spawnPoint.z);
        }
        else
        {
            transform.localPosition = new Vector3(spawnPoint.x, spawnPoint.y, randomSide);
        }

        agent.acceleration = 25f;
        agent.speed = 25f;
        animator.SetBool("Run", true);

        
        switch(angleState)
        {
            case AngleState.forward:
                NavMesh.SamplePosition(transform.position + (Vector3.forward * 100f), out  hit, 100f, NavMesh.AllAreas);
                break;
            case AngleState.back:
                NavMesh.SamplePosition(transform.position + (Vector3.back * 100f), out  hit, 100f, NavMesh.AllAreas);
                break;
            case AngleState.right:
                NavMesh.SamplePosition(transform.position + (Vector3.right * 100f), out  hit, 100f, NavMesh.AllAreas);
                break;
            case AngleState.left:
                NavMesh.SamplePosition(transform.position + (Vector3.left * 100f), out  hit, 100f, NavMesh.AllAreas);
                break;
        }
        agent.SetDestination(hit.position);


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isDamaged == false)
        {
            isDamaged = true;
            CharacterManager.Instance.Player.condition.uiconditions.health.Substract(damage);
        }
    }

    void Update()
    {
        if (agent.remainingDistance < 1.0f)
        {
            DestroyShadow();
        }
    }

    // ������ 2�� ���� ��
    public void InitObject(Vector3 _spawnPosition, float _maxDistance, bool _isTrueXFalseZ, float _dashSpeed, float _damage, AngleState _angleState)
    {
        spawnPoint = _spawnPosition;
        maxSideDistance = _maxDistance;
        isTrueXFalseZ = _isTrueXFalseZ;
        dashSpeed = _dashSpeed;
        damage = _damage;
        angleState = _angleState;
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
