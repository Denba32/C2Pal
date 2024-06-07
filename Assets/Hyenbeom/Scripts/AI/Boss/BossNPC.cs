using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossNPC : MonoBehaviour
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
    private BattlePattern pattern;

    // �뽬�� ���� ��
    private float dashCoolTime;
    public bool DashDamagedActive;
    public bool DontSetPlayerDestination;

    // ���Ÿ� ������ ���� ��   
    [Header("RangedAttack Settings")]
    public float rangedattackCooltime;
    public GameObject blackOrb;
    public Transform rangedattackTransform;

    // ����
    AIState aiState;

    // ��������
    public enum BattlePattern
    {
        None = -1,
        Scratch, // �÷��̾�� ����� ���
        Dash, // �÷��̾�� �߰Ÿ�
        RangedAttack, // �÷��̾� �� �� �ѹ���
        Rush = 10 // 2������ ���Ա�
    }

    // ������2
    bool isPhase2 = false;

    // SO
    public EnemySO statSO;

    // ���ݿ� �ʿ��� ��
    private float attackDelay;
    

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        scracthCollider.enabled = false; // �θ��� OnCollisionEnter�� �ڽ� Collider���� ������ ���� �� �ִ�..
        ChangeState(AIState.Idle);
    }

    void Update()
    {
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

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Attack:
                AttackingState();
                break;
        }
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
                // ���Ŀ� ��ü �����ų ����� ã�� ��... (�ƴϸ� ��� Trigger�� ������ �� ����.)
                Invoke("Destroy", 30f);
                break;
        }
    }

    public void ChangePattern(BattlePattern _pattern)
    {
        pattern = _pattern;
    }

    private void AttackingState()
    {
        // �÷��̾�� �����ٸ�, walk
        // �÷��̾�� �� �ִٸ�, run

        // ������ ������ ������ �Ŀ��� IdleState ���·� ��ȯ�� ��. (��Ÿ���� �ʿ��� ����)
        // �ٰŸ������� scracth (attack1 Ŭ��)
        // �ٰŸ����� ����� ������ ���� (attack2 Ŭ��)
        // ���Ÿ��� ���� �ѹ� �߻�ü �߻� (attack3 Ŭ��)

        // 2������� �Ѿ ��쿡�� ���� �޼��带 ȣ���� ��

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0) { attackDelay = 0; }
        }
        if (dashCoolTime > 0)
        {
            dashCoolTime -= Time.deltaTime;
            if (dashCoolTime <= 0) { dashCoolTime = 0; }
        }
        if (rangedattackCooltime > 0)
        {
            rangedattackCooltime -= Time.deltaTime;
            if (rangedattackCooltime <=0) { rangedattackCooltime = 0; }
        }

        // ������
        if (dashCoolTime <= 0 && playerDistance > statSO.attackDistance * statSO.attackDistance && playerDistance < dashDistance * dashDistance && IsPlayerInDashDirection())
        {
            animator.SetBool("Run", false); // ���ݰŸ� ����
            if (attackDelay <= 0)
            {
                animator.SetTrigger("Dash");
                agent.SetDestination(agent.destination * 1.5f); // �� �� �ָ� �پ��ִ� �� ���� ����..
                DontSetPlayerDestination = true;
                DashDamagedActive = true;
                attackDelay = 5f;
                //dashCoolTime = 20f;
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
                //CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.SetTrigger("Scracth");
                attackDelay = 5f;
            }
        }
        // ���Ÿ� ����
        else if (rangedattackCooltime <= 0 && playerDistance > dashDistance * dashDistance && playerDistance < rangedattackDistance * rangedattackDistance && IsPlayerInFieldOfView())
        {
            animator.SetBool("Run", false);
            if (attackDelay <= 0)
            {
                ChangePattern(BattlePattern.RangedAttack);
                animator.SetTrigger("RangedAttack");
                attackDelay = 3f;
                rangedattackCooltime = 15f;
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
                Debug.Log("�����⸦ ���߽��ϴ�.");
            }
        }
    }

    // ���Ÿ� ������ ���� ���� ��ü ����
    public void InstantiateBlackOrb()
    {
        Instantiate(blackOrb, rangedattackTransform.position, Quaternion.identity);
    }

    private void Enter2Phase() // �����ϴٸ� �ڷ�ƾ�� Ȱ���� ������...
    {
        // �� �߾ӿ� ������ ���� �ش� �������� �׸��ڵ��� ��ȯ�ؼ� �����Ŵ
        // �� �׸��ڿ��Դ� isTrigger�� �ο����� �� (�ε����� �о�� ������ ���� �ƴϴ� ������ ��)
        // �翬�� ��ġ�� ���ǻ��� ���ؼ� �ڽ����� �ּ� localposition�� �ٲ��� ��

        // �ش� �ൿ�� ������ ���� 2Phase�� ����
    }

    // ���Ŀ� IDamagable�� �ִ´ٸ� �ű⿡ �ǰ� �� ���� �� 2������ ������ ���� ��

    // �þ� Ȯ��
    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < statSO.fieldOfView * 0.5f;
    }

    private bool IsPlayerInDashDirection()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 20 * 0.5f;
    }

    // ���������� �Ѿ�ٸ� ȣ���� ��
    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
