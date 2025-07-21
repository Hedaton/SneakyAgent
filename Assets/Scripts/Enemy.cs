using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public Transform player;

    private NavMeshAgent agent;
    private Health health;
    private WeaponController weapon;

    private float lastSeenTimer = 0f;
    private float attackTimer = 0f;
    private Vector3 lastSeenPosition;
    private enum EnemyState { Wandering, Chasing, Attacking, Searching }
    private EnemyState state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        weapon = GetComponentInChildren<WeaponController>();
    }

    private void Start()
    {
        health.SetMaxHealth(enemyData.maxHealth);
        agent.speed = enemyData.moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        state = EnemyState.Wandering;
        SetNewWanderPoint();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= enemyData.detectionRange)
        {
            lastSeenPosition = player.position;
            lastSeenTimer = 5f;
            state = distance <= enemyData.attackRange ? EnemyState.Attacking : EnemyState.Chasing;
        }
        else if (lastSeenTimer > 0)
        {
            lastSeenTimer -= Time.deltaTime;
            if (state != EnemyState.Searching)
            {
                state = EnemyState.Searching;
            }
        }
        else
        {
            state = EnemyState.Wandering;
        }

        switch (state)
        {
            case EnemyState.Wandering:
                if (agent.remainingDistance < 0.5f)
                    SetNewWanderPoint();
                break;

            case EnemyState.Chasing:
                agent.SetDestination(player.position);
                break;
            case EnemyState.Attacking:
                agent.SetDestination(transform.position);
                Attack();
                break;
            case EnemyState.Searching:
                agent.SetDestination(lastSeenPosition);
                break;
        }
    }

    private void Attack()
    {
        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        transform.LookAt(player.position);

        if (enemyData.isRanged && weapon != null)
        {
            if (weapon.currentAmmo <= 0)
            {
                weapon.Reload();
            }
            else
            {
                weapon.Shoot();
                Debug.Log("Ranged Attack");
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, player.position) <= 2f)
            {
            Debug.Log("Melee");

            }
        }

            attackTimer = enemyData.attackCooldown;
        
    }

    private void SetNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }
}
