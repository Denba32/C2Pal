using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class BossShadow : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    public float dashSpeed;
    public float damage;
    private bool isDamaged;

    private IObjectPool<BossShadow> _ManagedPool;
    private Vector3 spawnPoint;    // ���� ���� (y�� ������ ����, x, z�� �˾Ƽ� �����ϵ���)
    private float maxSideDistance; // ��ȯ�� �� �¿� ����
    private bool isTrueXFalseZ;

    public AngleState angleState = AngleState.forward;

    private NavMeshHit hit;
    private NavMeshHit spawnHit;
    void Start()
    {
        RandomSpawn();
    }

    void OnEnable()
    {
        RandomSpawn();
        isDamaged = false;
    }

    void RandomSpawn()
    {
        float randomSide = Random.Range(-maxSideDistance, maxSideDistance);
        if (isTrueXFalseZ) // ���̸� x side false�� z side�Դϴ�.
        {
            NavMesh.SamplePosition(new Vector3(randomSide, spawnPoint.y, spawnPoint.z), out spawnHit, 3f, NavMesh.AllAreas);
        }
        else
        {
            NavMesh.SamplePosition(new Vector3(spawnPoint.x, spawnPoint.y, randomSide), out spawnHit, 3f, NavMesh.AllAreas);
        }

        transform.position = spawnHit.position;

        agent.acceleration = 25f;
        agent.speed = 25f;
        animator.SetBool("Run", true);

        SetThePath();
    }

    private void SetThePath()
    {
        float destinationDistance = 100f;
        while (true)
        {
            switch (angleState)
            {
                case AngleState.forward:
                    NavMesh.SamplePosition(transform.position + (Vector3.forward * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.back:
                    NavMesh.SamplePosition(transform.position + (Vector3.back * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.right:
                    NavMesh.SamplePosition(transform.position + (Vector3.right * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
                case AngleState.left:
                    NavMesh.SamplePosition(transform.position + (Vector3.left * destinationDistance), out hit, destinationDistance, NavMesh.AllAreas);
                    break;
            }
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path))
            {
                agent.SetDestination(new Vector3(hit.position.x, transform.position.y, hit.position.z));
                return;
            }
            destinationDistance -= 10f;
            if (destinationDistance == 0)
            {
                Debug.Log("������ �ĺ� �Ҵ�");
                return;
            }
            // ���� ���� �������� ������ �߻���. ���� z���� ª�Ƽ�; (���� ������ �ȵǴ� �����..;)
            // ��ֹ��� �ʹ� ���ٸ� ��ȯ���ڸ��� �ٷ� �����
            // y���� �ʹ� ũ�� ��߳��� ����� NavMesh�� �������� ���� (�׷��� �̷��� ����� AI�� �������� ���� �뵵�δ� ���� ����� ���� ���� �ɷ� �ľǵ�)
            // ����� y position�� ������ ��߳��� ���� ���� ���� ���� �ּ� z���� 45,
            // �� ���ذ� �ȵǴ� ���������, ��¿ �� ���� �޾Ƶ��̱�� ��.
            // �� �ڵ�� Ȥ���� �ٸ� ���� ������ �� �߸� ������ ��� �����ص�.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isDamaged == false)
        {
            isDamaged = true;
            CharacterManager.Instance.Player.condition.uiconditions.health.Substract(damage);
        }
    }

    void Update()
    {
        if (agent.remainingDistance < 1.0f)
        {
            DestroyShadow();
        }
    }

    // ������ 2�� ���� ��
    public void InitObject(Vector3 _spawnPosition, float _maxDistance, bool _isTrueXFalseZ, float _dashSpeed, float _damage, AngleState _angleState)
    {
        spawnPoint = _spawnPosition;
        maxSideDistance = _maxDistance;
        isTrueXFalseZ = _isTrueXFalseZ;
        dashSpeed = _dashSpeed;
        damage = _damage;
        angleState = _angleState;
    }

    public void SetManagedPool(IObjectPool<BossShadow> pool)
    {
        _ManagedPool = pool;
    }

    public void DestroyShadow()
    {
        _ManagedPool.Release(this);
    }
}
