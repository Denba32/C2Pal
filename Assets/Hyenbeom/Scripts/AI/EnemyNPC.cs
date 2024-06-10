using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour, IDamagable
{
    // �ʿ��� ������Ʈ
    [SerializeField] // ��� �ִϸ��̼��� �̰��� ���ؼ� �޾ƿ;���;; (��ǻ� SO�� ������ �ޱ� ����..)
    private Animator animator;
    NavMeshAgent agent;
    Collider _collider;
    // private SkinnedMeshRenderer[] meshRenderers;

    // �÷��̾� �Ÿ� ���ϱ�
    private float playerDistance;

    // ����
    AIState aiState;
    float currentHealth;
    float maxHealth;

    // SO
    public EnemySO statSO;

    // ���ݿ� �ʿ��� ��
    private float lastAttackTime;

    // ������������ ��ȯ�ƴٸ� �� ���� ����
    private GameObject spawnPoint;

    // ������ ������ �� ���
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
        // ���Ŀ� ������ ������ ������ �־��� ��
        if (aiState == AIState.Dead) { return; }
        // �÷��̾� ����
        if (CharacterManager.Instance.Player != null)
        {
            playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr�� Ȱ�������� ������ �ϴ� �� ���� ���ô�.
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

    // ���� ��ȯ
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
            case AIState.Dead: // �ʱ�ȭ �۾�
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                _collider.isTrigger = true; // ��� ��Ű�� �ϱ� ���ؼ�
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
            animator.SetBool("Moving", false); // ���ݰŸ��� ���� ���� ����ƥ �ʿ� ����.
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
        if (spawnPoint != null)
        {
            spawnPoint.GetComponent<MonsterSpawnPoint>().MonsterDead(this.gameObject);
        }
        foreach (GameObject item in dropItems)
        {
            if (item == null) { continue; } // �̹� �ֿ� �� ���� ������
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
