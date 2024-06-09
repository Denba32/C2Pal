using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Flee,
    Attack,
    Busy,
    GoingHome = 9,
    Dead = 10
} // 공격이나 다른 건 추후에 추가할 것

public class NPC : MonoBehaviour
{
    // 필요한 컴포넌트
    protected Animator animator;
    protected NavMeshAgent agent;
    protected Collider _collider;

    // private SkinnedMeshRenderer[] meshRenderers;

    // 플레이어 거리 구하기
    protected float playerDistance;

    // 상태
    protected AIState aiState;

    // SO
    public AnimalSO statSO;

    // 드롭을 위한 것
    public LayerMask dropableLayer;
    protected List<GameObject> dropItems;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }

    void Start() 
    {
        ChangeState(AIState.Wandering);
    }

    protected virtual void Update()
    {
        // 추후에 정지가 있으면 넣어줄 것
        if (aiState == AIState.Dead) { return; }
        // 플레이어 감지
        playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr을 활용했으니 제곱을 하는 걸 잊지 맙시다.

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                WanderingState();
                break;
            case AIState.Flee:
                FleeState();
                break;
        }

        animator.speed = agent.speed / statSO.walkSpeed;
    }

    // 상태 변환
    protected virtual void ChangeState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.isStopped = true;
                agent.speed = statSO.walkSpeed;
                break;
            case AIState.Wandering:
                agent.isStopped = false;
                agent.speed = statSO.walkSpeed;
                break;
            case AIState.Flee:
                agent.isStopped = false;
                agent.speed = statSO.runSpeed;
                break;
            case AIState.Dead: // 초기화 작업
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                _collider.isTrigger = true; // 통과 시키게 하기 위해서
                DropItem(); // 드랍 아이템
                StartCoroutine(DestroyMess());
                break;
        }
    }

    protected void WanderingState()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            ChangeState(AIState.Idle);
            Invoke("NewWanderingPoint", Random.Range(statSO.minWanderWaitTime, statSO.maxWanderWaitTime));
        }
        if (playerDistance < statSO.detectDistance * statSO.detectDistance)
        {
            ChangeState(AIState.Flee);
            agent.ResetPath();
        }
    }

    private void NewWanderingPoint()
    {
        if (aiState != AIState.Idle) return;

        ChangeState(AIState.Wandering);
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * statSO.detectDistance), out NavMeshHit hit, statSO.safeDistance, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    protected void FleeState()
    {
        float destinationDistance = (agent.destination - CharacterManager.Instance.Player.transform.position).sqrMagnitude;
        if (agent.remainingDistance < 1f || destinationDistance < statSO.safeDistance * statSO.safeDistance || playerDistance < 1f)
        {
            NewFleePoint();
        }
        else if (playerDistance > statSO.safeDistance * statSO.safeDistance)
        {
            agent.ResetPath();
            ChangeState(AIState.Wandering);
        }
    }

    private void NewFleePoint()
    {
        Vector3 vector = (transform.position - CharacterManager.Instance.Player.transform.position).normalized;
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (vector * statSO.safeDistance) + (Random.onUnitSphere * statSO.detectDistance), out hit, statSO.safeDistance + statSO.detectDistance, NavMesh.AllAreas);
        agent.SetDestination(hit.position);

        int i = 0;
        float destinationDistance = (agent.destination - CharacterManager.Instance.Player.transform.position).sqrMagnitude;
        while (GetDestinationAngle(hit.position) > 90 || destinationDistance < statSO.safeDistance * statSO.safeDistance) // 플레이어 방향 감지
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * statSO.safeDistance), out hit, statSO.safeDistance + statSO.detectDistance, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            destinationDistance = (agent.destination - CharacterManager.Instance.Player.transform.position).sqrMagnitude;
            i++;
            if (i == 10)
            {
                i = 0;
                break;
            }
        }

        agent.FindClosestEdge(out NavMeshHit edgeHit); // 구석 좌표 찾기

        if ((edgeHit.position - transform.position).sqrMagnitude < 1f) // 구석 탈출
        {
            vector = (transform.position - edgeHit.position).normalized; // 구석의 반대방향
            NavMesh.SamplePosition(transform.position + (vector * statSO.detectDistance) + (Random.onUnitSphere * statSO.safeDistance), out hit, statSO.safeDistance + statSO.detectDistance, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
        }
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - CharacterManager.Instance.Player.transform.position, transform.position + targetPos);
    }

    protected virtual void DropItem()
    {
        foreach (GameObject loot in statSO.dropOnDeath)
        {
            int i = 0;
            while (i < 30)
            {
                Vector3 spawnPoint = transform.position + Random.onUnitSphere;
                if (Physics.Raycast(transform.position + Vector3.up * 20, -transform.up, out RaycastHit hit, 30f, dropableLayer)) // 이 방법은 천장을 조심해야할 것..
                {
                    GameObject item = Instantiate(loot, new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z), Quaternion.identity);
                    dropItems.Add(item);
                    break;
                }
                i++;
            }
            
        }
    }

    protected virtual IEnumerator DestroyMess()
    {
        yield return new WaitForSeconds(15f);
        foreach (GameObject item in dropItems)
        {
            Destroy(item);
        }
        Destroy(this.gameObject);
    }
}
