using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

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

    // 패턴 전용
    private BattlePattern pattern;

    // 대쉬를 위한 것
    public bool DashDamagedActive;
    public bool DontSetPlayerDestination;
    private float currentDashCooltime;

    // 원거리 공격을 위한 것   
    [Header("RangedAttack Settings")]
    public Transform rangedattackTransform;
    private float currentRangedattackCooltime;

    // 페이즈 2를 위한 것
    [Header("Phase2Enter Skill")]
    public Vector3 usePosition;
    public Vector3 spawnShadowPosition;
    public float maxSideDistance;
    public bool isTrueXFalseZ;
    private IObjectPool<BossShadow> _Pool;

    // 이건 animator 접근용
    public bool activateSpawnShadow;

    // 상태
    AIState aiState;
    private float currentmagnification = 1f;

    // 공격패턴
    public enum BattlePattern
    {
        None = -1,
        Scratch, // 플레이어와 가까울 경우
        Dash, // 플레이어와 중거리
        RangedAttack, // 플레이어 멀 때 한번씩
        Rush = 10 // 2페이즈 진입기
    }

    // 공격 후 대기 시간
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
        scracthCollider.enabled = false; // 부모의 OnCollisionEnter는 자식 Collider에도 영향을 받을 수 있다..
        ChangeState(AIState.Idle);
        //Invoke("Enter2Phase", 10f); // 원래조건 체력이 반 이상 떨어졌을 때 발동시킬 것

        // 돌진기 같은 거 케어
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            agent.SetDestination(CharacterManager.Instance.Player.transform.position);
    }

    void Update()
    {
        // 추후에 정지가 있으면 넣어줄 것
        if (aiState == AIState.Dead || aiState == AIState.Busy) { return; }
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
            default:
                break;
        }

        animator.speed = currentmagnification;
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
                StopAllCoroutines();
                // 추후에 시체 통과시킬 방법을 찾을 것... (아니면 모든 Trigger를 꺼놓는 게 좋음.)
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
        // 2페이즈로 넘어갈 경우에는 따로 메서드를 호출할 것

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

        // 돌진기
        if (currentDashCooltime <= 0 && playerDistance > statSO.attackDistance * statSO.attackDistance && playerDistance < dashDistance * dashDistance && IsPlayerInDashDirection())
        {
                animator.SetBool("Run", false); // 공격거리 내부
            if (attackDelay <= 0)
            {
                animator.SetTrigger("Dash");
                agent.SetDestination(agent.destination * 1.5f); // 좀 더 멀리 뛰어주는 게 좋을 지도..
                DontSetPlayerDestination = true;
                DashDamagedActive = true;
                attackDelay = statSO.attackRate;
                currentDashCooltime = statSO.dashCoolTime - (statSO.attackRate * (currentmagnification - 1));
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
                attackDelay = statSO.attackRate  - (statSO.attackRate * (currentmagnification - 1));
            }
        }
        // 원거리 공격
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
        Instantiate(statSO.blackOrb, rangedattackTransform.position, Quaternion.identity);
    }

    private void Enter2Phase() // 가능하다면 코루틴을 활용할 생각임...
    {
        // 일단 첫번째 문제로는 모두 잠궈놓아야함.. 스폰이 끝나면 원래 상태로 돌아가고
        ChangePattern(BattlePattern.Rush);
        ChangeState(AIState.Busy);

        StartCoroutine(ReadyToRush());
    }



    IEnumerator ReadyToRush()
    {
        agent.SetDestination(usePosition + Vector3.back * 2);
        yield return new WaitUntil(() => agent.remainingDistance < 0.1f); // 이 단계에서 지멋대로 풀림;

        agent.SetDestination(transform.position + Vector3.forward * 2);

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
        // 돌진기 케어
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
            agent.SetDestination(CharacterManager.Instance.Player.transform.position);

        // 끝나고 나서
        ChangePattern(BattlePattern.None);
        ChangeState(AIState.Attack);
        animator.SetBool("Run", true);
        activateSpawnShadow = false;

        currentmagnification = statSO.Phase2Magnification;
    }

    // 추후에 IDamagable을 넣는다면 거기에 피가 반 깎였을 시 2페이즈 실행을 넣을 것

        // Shadow 오브젝트 풀 전용
    private BossShadow CreateShadow()
    {
        BossShadow bossShadow = Instantiate(statSO._ShadowPrefab).GetComponent<BossShadow>();
        bossShadow.InitObject(spawnShadowPosition, maxSideDistance, isTrueXFalseZ, statSO.shadowSpeed);
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


    // 시야 확인
    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < statSO.fieldOfView * 0.5f;
    }

        // 시야확인 (돌진용)
    private bool IsPlayerInDashDirection()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 20 * 0.5f;
    }

    // 스테이지가 넘어간다면 호출할 것
    protected virtual IEnumerator DestroyMess()
    {
        yield return new WaitForSeconds(90f);
        Destroy(this.gameObject);
    }
}
