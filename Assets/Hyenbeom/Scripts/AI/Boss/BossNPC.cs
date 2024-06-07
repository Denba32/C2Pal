using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossNPC : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField] // 몇몇 애니메이션은 이곳을 통해서 받아와야함;; (사실상 SO로 건네받기 힘듬..)
    private Animator animator;
    NavMeshAgent agent;
    // private SkinnedMeshRenderer[] meshRenderers;

    [Header("AttackPattern Settings")]
    [SerializeField] public Collider scracthCollider;

    // 플레이어 거리 구하기
    private float playerDistance;
    private float dashDistance = 15f;
    private float rangedattackDistance = 30f;
    private BattlePattern pattern;

    // 대쉬를 위한 것
    private float dashCoolTime;
    public bool DashDamagedActive;
    public bool DontSetPlayerDestination;

    // 원거리 공격을 위한 것   
    [Header("RangedAttack Settings")]
    public float rangedattackCooltime;
    public GameObject blackOrb;
    public Transform rangedattackTransform;

    // 상태
    AIState aiState;

    // 공격패턴
    public enum BattlePattern
    {
        None = -1,
        Scratch, // 플레이어와 가까울 경우
        Dash, // 플레이어와 중거리
        RangedAttack, // 플레이어 멀 때 한번씩
        Rush = 10 // 2페이즈 진입기
    }

    // 페이즈2
    bool isPhase2 = false;

    // SO
    public EnemySO statSO;

    // 공격에 필요한 것
    private float attackDelay;
    

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        scracthCollider.enabled = false; // 부모의 OnCollisionEnter는 자식 Collider에도 영향을 받을 수 있다..
        ChangeState(AIState.Idle);
    }

    void Update()
    {
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

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Attack:
                AttackingState();
                break;
        }
    }

    // 상태 변환
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
            case AIState.Dead: // 초기화 작업
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                // 추후에 시체 통과시킬 방법을 찾을 것... (아니면 모든 Trigger를 꺼놓는 게 좋음.)
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
        // 플레이어랑 가깝다면, walk
        // 플레이어랑 좀 멀다면, run

        // 각각의 공격을 시전한 후에는 IdleState 상태로 전환할 것. (쿨타임이 필요할 것임)
        // 근거리에서는 scracth (attack1 클립)
        // 근거리에서 벗어난다 싶으면 돌진 (attack2 클립)
        // 원거리일 때는 한번 발사체 발사 (attack3 클립)

        // 2페이즈로 넘어갈 경우에는 따로 메서드를 호출할 것

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

        // 돌진기
        if (dashCoolTime <= 0 && playerDistance > statSO.attackDistance * statSO.attackDistance && playerDistance < dashDistance * dashDistance && IsPlayerInDashDirection())
        {
            animator.SetBool("Run", false); // 공격거리 내부
            if (attackDelay <= 0)
            {
                animator.SetTrigger("Dash");
                agent.SetDestination(agent.destination * 1.5f); // 좀 더 멀리 뛰어주는 게 좋을 지도..
                DontSetPlayerDestination = true;
                DashDamagedActive = true;
                attackDelay = 5f;
                //dashCoolTime = 20f;
            }
        }
        // 할퀴기
        else if (playerDistance < statSO.attackDistance * statSO.attackDistance && IsPlayerInFieldOfView())
        {
            ChangeState(AIState.Idle);
            animator.SetBool("Run", false); // 공격거리 내부
            if (attackDelay <= 0)
            {
                ChangePattern(BattlePattern.Scratch);
                //CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.SetTrigger("Scracth");
                attackDelay = 5f;
            }
        }
        // 원거리 공격
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

    // 돌진 판정을 위한 OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pattern == BattlePattern.Dash && DashDamagedActive)
            {
                DashDamagedActive = false;
                Debug.Log("돌진기를 당했습니다.");
            }
        }
    }

    // 원거리 공격을 위한 검은 구체 생성
    public void InstantiateBlackOrb()
    {
        Instantiate(blackOrb, rangedattackTransform.position, Quaternion.identity);
    }

    private void Enter2Phase() // 가능하다면 코루틴을 활용할 생각임...
    {
        // 방 중앙에 적절히 서서 해당 방향으로 그림자들을 소환해서 통과시킴
        // 각 그림자에게는 isTrigger로 부여해줄 것 (부딪혀서 밀어내는 역할은 전혀 아니니 주의할 것)
        // 당연히 위치의 편의상을 위해서 자식으로 둬서 localposition을 바꿔줄 것

        // 해당 행동이 끝나고 나면 2Phase로 돌입
    }

    // 추후에 IDamagable을 넣는다면 거기에 피가 반 깎였을 시 2페이즈 실행을 넣을 것

    // 시야 확인
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

    // 스테이지가 넘어간다면 호출할 것
    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
