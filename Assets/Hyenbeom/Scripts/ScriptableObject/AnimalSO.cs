using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalStat", menuName = "SO/AnimalSO")]
public class AnimalSO : ScriptableObject
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

    // ��ȸ - ���� �Ÿ�, ������ �ּ� �ð�, �ִ� �ð� 
    [Header("Wander Settings")]
    public float detectDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // ���� - ���� �Ÿ�
    [Header("Flee Settings")]
    public float safeDistance;
}
