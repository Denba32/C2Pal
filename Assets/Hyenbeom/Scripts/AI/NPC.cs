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
} // �����̳� �ٸ� �� ���Ŀ� �߰��� ��

public class NPC : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    protected Animator animator;
    protected NavMeshAgent agent;
    protected Collider _collider;

    // private SkinnedMeshRenderer[] meshRenderers;

    // �÷��̾� �Ÿ� ���ϱ�
    protected float playerDistance;

    // ����
    protected AIState aiState;

    // SO
    public AnimalSO statSO;

    // ����� ���� ��
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
        // ���Ŀ� ������ ������ �־��� ��
        if (aiState == AIState.Dead) { return; }
        // �÷��̾� ����
        playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr�� Ȱ�������� ������ �ϴ� �� ���� ���ô�.

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

    // ���� ��ȯ
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
            case AIState.Dead: // �ʱ�ȭ �۾�
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                _collider.isTrigger = true; // ��� ��Ű�� �ϱ� ���ؼ�
                DropItem(); // ��� ������
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
        while (GetDestinationAngle(hit.position) > 90 || destinationDistance < statSO.safeDistance * statSO.safeDistance) // �÷��̾� ���� ����
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

        agent.FindClosestEdge(out NavMeshHit edgeHit); // ���� ��ǥ ã��

        if ((edgeHit.position - transform.position).sqrMagnitude < 1f) // ���� Ż��
        {
            vector = (transform.position - edgeHit.position).normalized; // ������ �ݴ����
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
                if (Physics.Raycast(transform.position + Vector3.up * 20, -transform.up, out RaycastHit hit, 30f, dropableLayer)) // �� ����� õ���� �����ؾ��� ��..
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
