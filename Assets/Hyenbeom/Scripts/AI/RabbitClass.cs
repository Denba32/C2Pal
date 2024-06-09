using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class RabbitClass : NPC
{
    private GameObject home;

    private bool isWantHome;

    protected override void Update()
    {
        if (aiState == AIState.Dead) { return; }
        // 플레이어 감지
        if (CharacterManager.Instance.Player != null)
        {
            playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr을 활용했으니 제곱을 하는 걸 잊지 맙시다.
        }
        else
        {
            playerDistance = statSO.safeDistance * statSO.safeDistance + 1f;
        }

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
            case AIState.GoingHome:
                break;
        }

        animator.speed = agent.speed / statSO.walkSpeed;
    }

    protected override void ChangeState(AIState state)
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
            case AIState.GoingHome:
                agent.isStopped = false;
                agent.speed = statSO.walkSpeed;
                break;
            case AIState.Dead: // 초기화 작업
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                _collider.isTrigger = true; // 통과 시키게 하기 위해서
                StartCoroutine(DestroyMess());
                DropItem(); // 드랍 아이템
                break;
        }
    }

    public void SetHome(GameObject rabbitHole)
    {
        home = rabbitHole;
    }

    public void DontWantHome()
    {
        isWantHome = false;
        agent.ResetPath();
        ChangeState(AIState.Wandering);
    }

    public void GoHome()
    {
        if (aiState == AIState.Dead) { return; } // 죽으면 못 돌아감
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(home.transform.position, path))
        {
            isWantHome = true;
            ChangeState(AIState.GoingHome);
            NavMesh.SamplePosition(home.transform.position, out NavMeshHit hit, statSO.detectDistance, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
        }
    }

    // 집에 닿을 시
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (aiState == AIState.Dead) { return; }
            ChangeState(AIState.Dead);
        }
        if (collision.gameObject == home)
        {
            if (isWantHome == true && aiState != AIState.Dead)
            {
                home.GetComponent<RabbitHole>().RabbitGoHome(this.gameObject);
            }
        }
    }

    protected override IEnumerator DestroyMess()
    {
        yield return new WaitForSeconds(15f);
        if (home != null)
        {
            home.GetComponent<RabbitHole>().RabbitDead(this.gameObject);
        }
        foreach (GameObject item in dropItems)
        {
            if (item == null) { continue; } // 이미 주운 거 오류 방지용
            Destroy(item);
        }
        Destroy(this.gameObject);
    }
}
