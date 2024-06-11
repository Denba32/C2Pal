using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "BossSO", menuName = "SO/BossSO")]
public class BossSO : ScriptableObject
{
    // ������Ʈ
    public Animator animator;

    // ����
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;

    // ����
    [Header("Loot")]
    public GameObject[] dropOnDeath;

    // ���� -  ���� �Ÿ�
    [Header("Wander Settings")]
    public float detectDistance;

    // ���� - ���ݷ�, ���ݼӵ�, ���ݰŸ�, �þ�
    [Header("Combat Settings")]
    public int damage;
    public float attackRate;
    public float attackDistance;
    public float fieldOfView;

    // �뽬�� ���� ��
    [Header("Dash Settings")]
    public float dashCoolTime;

    // ���Ÿ� ������ ���� ��   
    [Header("RangedAttack Settings")]
    public float rangedattackCooltime;
    public GameObject blackOrb;

    // ������ 2�� ���� ��
    [Header("Phase2Enter Skill")]
    public GameObject _ShadowPrefab;
    public int _maxSize;
    public float spawnDuration;
    public int spawnAmount;
    public float shadowSpeed;
    public float Phase2Magnification;
}
