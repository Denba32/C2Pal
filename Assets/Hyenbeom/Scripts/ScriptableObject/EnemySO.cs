using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat", menuName = "SO/EnemySO")]
public class EnemySO : ScriptableObject
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

    // ��ȸ -  ���� �Ÿ�, ������ �ּ� �ð�, �ִ� �ð� 
    [Header("Wander Settings")]
    public float detectDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // ���� - ���ݷ�, ���ݼӵ�, ���ݰŸ�, �þ�
    [Header("Combat Settings")]
    public int damage;
    public float attackRate;
    public float attackDistance;
    public float fieldOfView;
}
