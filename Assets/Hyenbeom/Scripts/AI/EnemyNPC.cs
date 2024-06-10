using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour, IDamagable
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
    float currentHealth;
    float maxHealth;

    // SO
    public EnemySO statSO;

    // 공격에 필요한 것
    private float lastAttackTime;

    // 스폰지점에서 소환됐다면 내 스폰 지점
    private GameObject spawnPoint;

    // 아이템 제거할 때 사용
    public LayerMask dropableLayer;
    protected List<GameObject> dropItems = new List<GameObject>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
    }

    void Start()
    {
        currentHealth = statSO.health;
        maxHealth = statSO.health;
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
            case AIState.Hit:
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
                DropItem();
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
        if (playerDistance < statSO.detectDistance * statSO.detectDistance && IsPlayerInFieldOfView() || aiState == AIState.Hit)
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

                if(!CharacterManager.Instance.Player.controller.isInvincible)
                    CharacterManager.Instance.Player.condition.uiconditions.health.Substract(statSO.damage);

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

    protected virtual void DropItem()
    {
        foreach (GameObject loot in statSO.dropOnDeath)
        {
            int i = 0;
            while (i < 5)
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
        if (spawnPoint != null)
        {
            spawnPoint.GetComponent<MonsterSpawnPoint>().MonsterDead(this.gameObject);
        }
        foreach (GameObject item in dropItems)
        {
            if (item == null) { continue; } // 이미 주운 거 오류 방지용
            Destroy(item);
        }
        Destroy(this.gameObject);
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth > 0f)
        {
            ChangeState(AIState.Hit);
        }
        else if (currentHealth <= 0f)
        {
            ChangeState(AIState.Dead);
        }
    }
}
