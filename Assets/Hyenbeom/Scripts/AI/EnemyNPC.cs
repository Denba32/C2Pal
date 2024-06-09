using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField] // 몇몇 애니메이션은 이곳을 통해서 받아와야함;; (사실상 SO의 도움을 받기 힘듬..)
    private Animator animator;
    NavMeshAgent agent;
    Collider _collider;
    // private SkinnedMeshRenderer[] meshRenderers;

    // 플레이어 거리 구하기
    private float playerDistance;

    // 상태
    AIState aiState;

    // SO
    public EnemySO statSO;

    // 공격에 필요한 것
    private float lastAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
    }

    void Start()
    {
        ChangeState(AIState.Wandering);
    }

    void Update()
    {
        // 추후에 정지가 있으면 정지도 넣어줄 것
        if (aiState == AIState.Dead) { return; }
        // 플레이어 감지
        if (CharacterManager.Instance.Player != null)
        {
            playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr을 활용했으니 제곱을 하는 걸 잊지 맙시다.
        }
        else
        {
            playerDistance = statSO.detectDistance * statSO.detectDistance + 1f;
        }

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                WanderingState();
                break;
            case AIState.Attack:
                AttackingState();
                break;
        }

        animator.speed = agent.speed / statSO.walkSpeed;
    }

    // 상태 변환
    void ChangeState(AIState state)
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
            case AIState.Attack:
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
                StartCoroutine(DestroyMess());
                break;
        }
    }

    void WanderingState()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            ChangeState(AIState.Idle);
            Invoke("NewWanderingPoint", Random.Range(statSO.minWanderWaitTime, statSO.maxWanderWaitTime));
        }
        if (playerDistance < statSO.detectDistance * statSO.detectDistance && IsPlayerInFieldOfView())
        {
            ChangeState(AIState.Attack);
            agent.ResetPath();
        }
    }

    private void NewWanderingPoint()
    {
        if (aiState != AIState.Idle) return;

        ChangeState(AIState.Wandering);
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * statSO.detectDistance), out NavMeshHit hit, statSO.detectDistance, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    private void AttackingState()
    {
        if (playerDistance < statSO.attackDistance * statSO.attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            animator.SetBool("Moving", false); // 공격거리에 있을 때는 통통튈 필요 없다.
            if (Time.time - lastAttackTime > statSO.attackRate)
            {
                lastAttackTime = Time.time;
                //CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if(playerDistance < statSO.detectDistance * statSO.detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if(agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    if (playerDistance > statSO.attackDistance * statSO.attackDistance) { animator.SetBool("Moving", true); }

                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    ChangeState(AIState.Wandering);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                ChangeState(AIState.Wandering);
            }
        }
    }

    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < statSO.fieldOfView * 0.5f;
    }

    protected virtual IEnumerator DestroyMess()
    {
        yield return new WaitForSeconds(15f);
        Destroy(this.gameObject);
    }
}
