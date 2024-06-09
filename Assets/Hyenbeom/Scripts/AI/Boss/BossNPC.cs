using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public enum AngleState
{
    forward,
    back,
    left,
    right
}

public class BossNPC : MonoBehaviour, IDamagable
{
    // �ʿ��� ������Ʈ
    [SerializeField] // ��� �ִϸ��̼��� �̰��� ���ؼ� �޾ƿ;���;; (��ǻ� SO�� �ǳ׹ޱ� ����..)
    private Animator animator;
    NavMeshAgent agent;
    // private SkinnedMeshRenderer[] meshRenderers;

    [Header("AttackPattern Settings")]
    [SerializeField] public Collider scracthCollider;

    // �÷��̾� �Ÿ� ���ϱ�
    private float playerDistance;
    private float dashDistance = 15f;
    private float rangedattackDistance = 30f;

    // ���� ����
    private BattlePattern pattern;

    // �뽬�� ���� ��
    public bool DashDamagedActive;
    public bool DontSetPlayerDestination;
    private float currentDashCooltime;

    // ���Ÿ� ������ ���� ��   
    [Header("RangedAttack Settings")]
    public Transform rangedattackTransform;
    private float currentRangedattackCooltime;

    // ������ 2�� ���� ��
    [Header("Phase2Enter Skill")]
    public Vector3 usePosition;
    public Vector3 spawnShadowPosition;
    public float maxSideDistance;
    public bool isTrueXFalseZ;
    private IObjectPool<BossShadow> _Pool;
    public AngleState forwardAngle;

    // �̰� animator ���ٿ�
    public bool activateSpawnShadow;

    // ����
    AIState aiState;
    private float currentmagnification = 1f;
    private bool isPhaseTwo = false;
    float currentHealth;
    float maxHealth;

    // ��������
    public enum BattlePattern
    {
        None = -1,
        Scratch, // �÷��̾�� ����� ���
        Dash, // �÷��̾�� �߰Ÿ�
        RangedAttack, // �÷��̾� �� �� �ѹ���
        Rush = 10 // 2������ ���Ա�
    }

    // ���� �� ��� �ð�
    private float attackDelay;

    // SO
    public BossSO statSO;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _Pool = new ObjectPool<BossShadow>(CreateShadow, OnGetShadow, OnReleaseShadow, OnDestroyShadow, maxSize:statSO._maxSize);
    }

    void Start()
    {
        currentHealth = statSO.health;
        maxHealth = statSO.health;
        scracthCollider.enabled = false; // �θ��� OnCollisionEnter�� �ڽ� Collider���� ������ ���� �� �ִ�..
        ChangeState(AIState.Idle);
        Invoke("Enter2Phase", 10f); // ������ ���� �׽�Ʈ��

        // ������ ���� �� �ɾ�
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            agent.SetDestination(CharacterManager.Instance.Player.transform.position);
    }

    void Update()
    {
        // ���Ŀ� ������ ������ �־��� ��
        if (aiState == AIState.Dead || pattern == BattlePattern.Rush) { return; }
        // �÷��̾� ����
        if (CharacterManager.Instance.Player != null)
        {
            playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr�� Ȱ�������� ������ �ϴ� �� ���� ���ô�.
        }
        else
        {
            playerDistance = statSO.detectDistance * statSO.detectDistance + 1f;
        }

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Attack:
                AttackingState();
                break;
            default:
                break;
        }

        animator.speed = currentmagnification;
    }

    // ���� ��ȯ
    public void ChangeState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.isStopped = true;
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
                StopAllCoroutines();
                // ���Ŀ� ��ü �����ų ����� ã�� ��... (�ƴϸ� ��� Trigger�� ������ �� ����.)
                StartCoroutine(DestroyMess());
                break;
            default:
                break;
        }
    }

    public void ChangePattern(BattlePattern _pattern)
    {
        pattern = _pattern;

        switch(pattern)
        {
            case BattlePattern.Rush:
                animator.SetBool("Run", true);
                agent.isStopped = false;
                agent.speed = statSO.runSpeed;
                break;
            default:
                break;
        }
    }

    private void AttackingState()
    {
        // 2������� �Ѿ ��쿡�� ���� �޼��带 ȣ���� ��

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0) { attackDelay = 0; }
        }
        if (currentDashCooltime > 0)
        {
            currentDashCooltime -= Time.deltaTime;
            if (currentDashCooltime <= 0) { currentDashCooltime = 0; }
        }
        if (currentRangedattackCooltime > 0)
        {
            currentRangedattackCooltime -= Time.deltaTime;
            if (currentRangedattackCooltime <= 0) { currentRangedattackCooltime = 0; }
        }

        // ������
        if (currentDashCooltime <= 0 && playerDistance > statSO.attackDistance * statSO.attackDistance && playerDistance < dashDistance * dashDistance && IsPlayerInDashDirection())
        {
                animator.SetBool("Run", false); // ���ݰŸ� ����
            if (attackDelay <= 0)
            {
                animator.SetTrigger("Dash");
                agent.SetDestination(agent.destination * 1.5f); // �� �� �ָ� �پ��ִ� �� ���� ����..
                DontSetPlayerDestination = true;
                DashDamagedActive = true;
                attackDelay = statSO.attackRate;
                currentDashCooltime = statSO.dashCoolTime - (statSO.attackRate * (currentmagnification - 1));
            }
        }
        // ������
        else if (playerDistance < statSO.attackDistance * statSO.attackDistance && IsPlayerInFieldOfView())
        {
            ChangeState(AIState.Idle);
            animator.SetBool("Run", false); // ���ݰŸ� ����
            if (attackDelay <= 0)
            {
                ChangePattern(BattlePattern.Scratch);
                animator.SetTrigger("Scratch");
                attackDelay = statSO.attackRate  - (statSO.attackRate * (currentmagnification - 1));
            }
        }
        // ���Ÿ� ����
        else if (currentRangedattackCooltime <= 0 && playerDistance > dashDistance * dashDistance && playerDistance < rangedattackDistance * rangedattackDistance && IsPlayerInFieldOfView())
        {
            animator.SetBool("Run", false);
            if (attackDelay <= 0)
            {
                ChangePattern(BattlePattern.RangedAttack);
                animator.SetTrigger("RangedAttack");
                attackDelay = statSO.attackRate - 2f - (statSO.attackRate * (currentmagnification - 1));
                currentRangedattackCooltime = statSO.rangedattackCooltime;
            }
        }
        else
        {
            if (DontSetPlayerDestination) { return; }
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            {
                if (attackDelay <= 0)
                {
                    animator.SetBool("Run", true);
                    ChangeState(AIState.Attack);
                }
                agent.SetDestination(CharacterManager.Instance.Player.transform.position);
            }

        }
    }

    // ���� ������ ���� OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pattern == BattlePattern.Dash && DashDamagedActive)
            {
                DashDamagedActive = false;
                CharacterManager.Instance.Player.condition.uiconditions.health.Substract(statSO.damage);
            }
        }
    }

    // ���Ÿ� ������ ���� ���� ��ü ����
    public void InstantiateBlackOrb()
    {
        Instantiate(statSO.blackOrb, rangedattackTransform.position, Quaternion.identity);
    }

    private void Enter2Phase() // �����ϴٸ� �ڷ�ƾ�� Ȱ���� ������...
    {
        // �ϴ� ù��° �����δ� ��� ��ų��ƾ���.. ������ ������ ���� ���·� ���ư���
        ChangePattern(BattlePattern.Rush);
        ChangeState(AIState.Busy);

        StartCoroutine(ReadyToRush());
    }



    IEnumerator ReadyToRush()
    {
        switch (forwardAngle)   // ���� ��ȯ ��ȯ��
        {
            case AngleState.forward:
                agent.SetDestination(usePosition - Vector3.forward * 2);
                break;
            case AngleState.back:
                agent.SetDestination(usePosition - Vector3.back * 2);
                break;
            case AngleState.left:
                agent.SetDestination(usePosition - Vector3.left * 2);
                break;
            case AngleState.right:
                agent.SetDestination(usePosition - Vector3.right * 2);
                break;
        }
        yield return new WaitUntil(() => agent.remainingDistance < 0.1f);

        switch (forwardAngle)   // ���� ��ȯ ��ȯ��
        {
            case AngleState.forward:
                agent.SetDestination(transform.position + Vector3.forward * 2);
                break;
            case AngleState.back:
                agent.SetDestination(transform.position + Vector3.back * 2);
                break;
            case AngleState.left:
                agent.SetDestination(transform.position + Vector3.left * 2);
                break;
            case AngleState.right:
                agent.SetDestination(transform.position + Vector3.right * 2);
                break;
        }
        yield return new WaitUntil(() => agent.remainingDistance < 0.05f);

        animator.SetBool("Run", false);
        agent.isStopped = true;

        animator.SetTrigger("Phase2EnterTrigger");
        yield return new WaitUntil(() => activateSpawnShadow);
        StartCoroutine(SpawnShadow());
    }

    IEnumerator SpawnShadow()
    {
        int i = 0;
        while(i < statSO.spawnAmount)
        {
            yield return new WaitForSeconds(statSO.spawnDuration);
            _Pool.Get();
            i++;
        }
        // ������ �ɾ�
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            agent.SetDestination(CharacterManager.Instance.Player.transform.position);

        // ������ ����
        ChangePattern(BattlePattern.None);
        ChangeState(AIState.Attack);
        animator.SetBool("Run", true);
        activateSpawnShadow = false;

        currentmagnification = statSO.Phase2Magnification;
    }

    // ���Ŀ� IDamagable�� �ִ´ٸ� �ű⿡ �ǰ� �� ���� �� 2������ ������ ���� ��

        // Shadow ������Ʈ Ǯ ����
    private BossShadow CreateShadow()
    {
        BossShadow bossShadow = Instantiate(statSO._ShadowPrefab).GetComponent<BossShadow>();
        bossShadow.InitObject(spawnShadowPosition, maxSideDistance, isTrueXFalseZ, statSO.shadowSpeed, statSO.damage, forwardAngle);
        bossShadow.SetManagedPool(_Pool);
        return bossShadow;
    }

    private void OnGetShadow(BossShadow bossShadow)
    {
        bossShadow.gameObject.SetActive(true);
    }

    private void OnReleaseShadow(BossShadow bossShadow)
    {
        bossShadow.gameObject.SetActive(false);
    }

    private void OnDestroyShadow(BossShadow bossShadow)
    {
        Destroy(bossShadow.gameObject);
    }


    // �þ� Ȯ��
    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < statSO.fieldOfView * 0.5f;
    }

        // �þ�Ȯ�� (������)
    private bool IsPlayerInDashDirection()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 20 * 0.5f;
    }

        // �ı���
    protected virtual IEnumerator DestroyMess()
    {
        yield return new WaitForSeconds(90f);
        Destroy(this.gameObject);
    }

    public void Damage(float damage)
    {
        if (pattern == BattlePattern.Rush) { Debug.Log("������ ���� �� ����"); }
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= maxHealth * 0.5f && !isPhaseTwo)
        {
            isPhaseTwo = true;
            Enter2Phase();
        }
        else if (currentHealth <= 0f)
        {
            ChangeState(AIState.Dead);
        }
    }
}
