using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat", menuName = "SO/EnemySO")]
public class EnemySO : ScriptableObject
{
    // 컴포넌트
    public Animator animator;

    // 스텟
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;

    // 보상
    [Header("Loot")]
    public GameObject[] dropOnDeath;

    // 배회 -  감지 거리, 원더링 최소 시간, 최대 시간 
    [Header("Wander Settings")]
    public float detectDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // 전투 - 공격력, 공격속도, 공격거리, 시야
    [Header("Combat Settings")]
    public int damage;
    public float attackRate;
    public float attackDistance;
    public float fieldOfView;
}
