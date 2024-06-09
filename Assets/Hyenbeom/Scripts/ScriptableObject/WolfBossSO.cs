using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "BossSO", menuName = "SO/BossSO")]
public class BossSO : ScriptableObject
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

    // 감지 -  감지 거리
    [Header("Wander Settings")]
    public float detectDistance;

    // 전투 - 공격력, 공격속도, 공격거리, 시야
    [Header("Combat Settings")]
    public int damage;
    public float attackRate;
    public float attackDistance;
    public float fieldOfView;

    // 대쉬를 위한 것
    [Header("Dash Settings")]
    public float dashCoolTime;

    // 원거리 공격을 위한 것   
    [Header("RangedAttack Settings")]
    public float rangedattackCooltime;
    public GameObject blackOrb;

    // 페이즈 2를 위한 것
    [Header("Phase2Enter Skill")]
    public GameObject _ShadowPrefab;
    public int _maxSize;
    public float spawnDuration;
    public int spawnAmount;
    public float shadowSpeed;
    public float Phase2Magnification;
}
