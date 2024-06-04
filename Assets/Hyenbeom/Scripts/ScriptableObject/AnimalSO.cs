using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalStat", menuName = "SO/AnimalSO")]
public class AnimalSO : ScriptableObject
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

    // 배회 - 원더링 최소 시간, 최대 시간 // 애초부터 랜덤인데 최소 거리, 최대 거리가 필요할까 싶네..
    [Header("Wander Settings")]
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // 도주 - 플레이어 감지 거리, 안전 거리
    [Header("Flee Settings")]
    public float detectDistance;
    public float safeDistance;
}
