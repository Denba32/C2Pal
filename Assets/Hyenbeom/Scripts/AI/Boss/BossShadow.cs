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
    public float damage;
    private bool isDamaged;

    private IObjectPool<BossShadow> _ManagedPool;
    private Vector3 spawnPoint;    // 스폰 시점 (y는 무조건 고정, x, z는 알아서 설정하도록)
    private float maxSideDistance; // 소환할 때 좌우 시점
    private bool isTrueXFalseZ;

    public AngleState angleState = AngleState.forward;

    private NavMeshHit hit;
    private NavMeshHit spawnHit;
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
        if (isTrueXFalseZ) // 참이면 x side false면 z side입니다.
        {
            NavMesh.SamplePosition(new Vector3(randomSide, spawnPoint.y, spawnPoint.z), out spawnHit, 3f, NavMesh.AllAreas);
        }
        else
        {
            NavMesh.SamplePosition(new Vector3(spawnPoint.x, spawnPoint.y, randomSide), out spawnHit, 3f, NavMesh.AllAreas);
        }

        transform.position = spawnHit.position;

        agent.acceleration = 25f;
        agent.speed = 25f;
        animator.SetBool("Run", true);

        SetThePath();
    }

    private void SetThePath()
    {
        float destinationDistance = 100f;
        while (true)
        {
            switch (angleState)
            {
                case AngleState.forward:
                    NavMesh.SamplePosition(transform.position + (Vector3.forward * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.back:
                    NavMesh.SamplePosition(transform.position + (Vector3.back * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.right:
                    NavMesh.SamplePosition(transform.position + (Vector3.right * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.left:
                    NavMesh.SamplePosition(transform.position + (Vector3.left * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
            }
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path))
            {
                agent.SetDestination(new Vector3(hit.position.x, transform.position.y, hit.position.z));
                return;
            }
            destinationDistance -= 10f;
            if (destinationDistance == 0)
            {
                Debug.Log("목적지 식별 불능");
                return;
            }
            // 위와 같이 했음에도 오류가 발생함. 땅의 z축이 짧아서; (제일 납득이 안되는 결과임..;)
            // 장애물이 너무 많다면 소환하자마자 바로 사라짐
            // y축이 너무 크게 어긋난도 제대로 NavMesh를 감지하지 못함 (그래서 이러한 방식의 AI는 공중으로 나는 용도로는 전혀 써먹을 수가 없는 걸로 파악됨)
            // 결론은 y position의 생성의 어긋남은 작을 수록 좋고 땅의 최소 z축은 45,
            // 좀 이해가 안되는 결과이지만, 어쩔 수 없이 받아들이기로 함.
            // 위 코드는 혹여나 다른 땅을 집었을 때 잘못 집었을 까봐 설정해둠.
        }
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

    // 페이즈 2를 위한 것
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
