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
        // �÷��̾� ����
        if (CharacterManager.Instance.Player != null)
        {
            playerDistance = (transform.position - CharacterManager.Instance.Player.transform.position).sqrMagnitude; // sqr�� Ȱ�������� ������ �ϴ� �� ���� ���ô�.
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
            case AIState.Dead: // �ʱ�ȭ �۾�
                agent.isStopped = true;
                agent.speed = 0f;
                animator.speed = 1f;
                agent.ResetPath();
                animator.SetBool("Dead", true);
                _collider.isTrigger = true; // ��� ��Ű�� �ϱ� ���ؼ�
                StartCoroutine(DestroyMess());
                DropItem(); // ��� ������
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
        if (aiState == AIState.Dead) { return; } // ������ �� ���ư�
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(home.transform.position, path))
        {
            isWantHome = true;
            ChangeState(AIState.GoingHome);
            NavMesh.SamplePosition(home.transform.position, out NavMeshHit hit, statSO.detectDistance, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
        }
    }

    // ���� ���� ��
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
            if (item == null) { continue; } // �̹� �ֿ� �� ���� ������
            Destroy(item);
        }
        Destroy(this.gameObject);
    }
}
