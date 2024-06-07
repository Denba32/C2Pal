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

    // 공격 패턴 1
    public void EnableScracth()
    {
        scracthCollider.enabled = true;
    }

    public void DisAbleScratch()
    {
        scracthCollider.enabled = false;
    }

    // 추후에 SO로 반영해줄 것!

        // 돌진 공격
    public void ReadyDash() // 그냥 impulse 넣어줘야하나 이 생각 중...
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
        bossScript.ChangePattern(BattlePattern.None); // 부딪히는 판정을 없애야한다.

        // 원래 상태로
        bossScript.DashDamagedActive = false;
        agent.angularSpeed = 1000;
        agent.acceleration = 8;
        bossScript.DontSetPlayerDestination = false;
        bossScript.ChangeState(AIState.Idle);
    }

    // 원거리 공격

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
