using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BossNPC;

public class ActivatePattern : MonoBehaviour
{
    public Collider scracthCollider;
    public NavMeshAgent agent;
    public BossNPC bossScript;

    // ���� ���� 1
    public void EnableScracth()
    {
        scracthCollider.enabled = true;
    }

    public void DisAbleScratch()
    {
        scracthCollider.enabled = false;
    }

    // ���Ŀ� SO�� �ݿ����� ��!

        // ���� ����
    public void ReadyDash() // �׳� impulse �־�����ϳ� �� ���� ��...
    {
        agent.isStopped = true;
        agent.acceleration = 500;
        agent.speed = 50;
        agent.autoBraking = false;
    }
    
    public void StartDash()
    {
        bossScript.ChangePattern(BattlePattern.Dash);
        agent.isStopped = false;
        agent.angularSpeed = 0;
        
    }

    public void EndDash()
    {
        agent.autoBraking = true;
        bossScript.ChangePattern(BattlePattern.None); // �ε����� ������ ���־��Ѵ�.

        // ���� ���·�
        bossScript.DashDamagedActive = false;
        agent.angularSpeed = 1000;
        agent.acceleration = 8;
        bossScript.DontSetPlayerDestination = false;
        bossScript.ChangeState(AIState.Idle);
    }

    // ���Ÿ� ����

    public void ReadyRangedAttack()
    {
        agent.isStopped = true;
    }

    public void FireOrb()
    {
        bossScript.InstantiateBlackOrb();
        agent.isStopped = false;
        bossScript.ChangeState(AIState.Idle);
    }
}
