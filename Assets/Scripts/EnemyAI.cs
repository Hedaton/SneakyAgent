using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent için bu kütüphane gerekli

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public enum AIState
    {
        Patrol, Chase, Attack, Search
    }
    [SerializeField] private AIState currentState = AIState.Patrol;

    public enum AttackType
    {
        Melee, Ranged
    }
    [Header("Attack Type")]
    [SerializeField] private AttackType attackType = AttackType.Melee;

    [Header("References")]
    [SerializeField] private WeaponController weaponController; // for ranged type
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    private NavMeshAgent agent;
    private Animator animator;
    private Health health;

    [Header("Detection Settings")]
    [SerializeField] private float visionConeRange = 15f;
    [Range(0, 360)]
    [SerializeField] private float visionConeAngle = 90f;

    [Header("Attack Settings")]
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float optimalShootingRange = 10f;
    [SerializeField] private float minimumShootingRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    [Header("Patrol Settings")]
    [SerializeField] private Vector3 patrolAreaCenter;
    [SerializeField] private float patrolAreaRadius = 20f;
    [SerializeField] private Vector2 patrolReconsiderTimeRange;
    private float patrolTimer = 0f;
    private float currentReconsiderDuration;

    [Header("Search Settings")]
    private Vector3 lastSeenPosition;
    private float searchTimer = 0f;
    [SerializeField] private float searchDuration = 5f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += HandleDamage;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= HandleDamage;
        }
    }

    private void HandleDamage(float currentHealth, float maxHealth)
    {
        if (currentState == AIState.Patrol)
        {
            lastSeenPosition = player.position;
            SwitchState(AIState.Search);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Search:
                SearchBehavior();
                break;
        }
    }

    private void PatrolBehavior()
    {
        if (IsPlayerDetected())
        {
            SwitchState(AIState.Chase);
            return;
        }

        patrolTimer += Time.deltaTime;

        if (patrolTimer > currentReconsiderDuration)
        {
            SetNewRandomPatrolPoint();
        }

        // Animasyon
        // animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void ChaseBehavior()
    {
        if (!IsPlayerDetected())
        {
            SwitchState(AIState.Search);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (attackType == AttackType.Melee)
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                SwitchState(AIState.Attack);
                return;
            }
            agent.SetDestination(player.position);

        }
        else
        {
            agent.isStopped = false;
            if (distanceToPlayer > optimalShootingRange)
            {
                agent.SetDestination(player.position);
            }
            else if (distanceToPlayer < minimumShootingRange)
            {
                Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
                Vector3 retreatPosition = transform.position + directionAwayFromPlayer * Random.Range(0, minimumShootingRange);
                agent.SetDestination(retreatPosition);
            }
            else
            {
                agent.isStopped = true;
                SwitchState(AIState.Attack);
                return;
            }
        }


        lastSeenPosition = player.position;
        // animator.SetFloat("Speed", agent.velocity.magnitude);
    }

  private void AttackBehavior()
    {
        agent.isStopped = true;
        transform.LookAt(player.position);

      
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

 
        if (attackType == AttackType.Melee && distanceToPlayer > meleeAttackRange)
        {
            SwitchState(AIState.Chase);
            return;
        }

      
        if (attackType == AttackType.Ranged && (distanceToPlayer < minimumShootingRange || distanceToPlayer > optimalShootingRange))
        {
            SwitchState(AIState.Chase);
            return;
        }


        if (!IsPlayerDetected())
        {
            SwitchState(AIState.Search);
            return;
        }


        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (attackType == AttackType.Melee)
            {
                Debug.Log("Melee Attack!");
            }
            else if (weaponController != null)
            {
          
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                weaponController.ShootInDirection(transform.position, directionToPlayer);
            }
            
            lastAttackTime = Time.time;
        }
    }

    private void SearchBehavior()
    {
        // Ararken oyuncuyu tekrar gördü mü?
        if (IsPlayerDetected())
        {
            SwitchState(AIState.Chase);
            return;
        }

        searchTimer += Time.deltaTime;

        // Arama süresi dolduysa devriyeye dön
        if (searchTimer > searchDuration)
        {
            SwitchState(AIState.Patrol);
        }
    }

    // --- YARDIMCI METOTLAR ---

    private void SwitchState(AIState newState)
    {
        if (currentState == newState) return;

        // State'ten çıkış logic'i (gerekiyorsa)

        currentState = newState;

        // State'e giriş logic'i
        switch (currentState)
        {
            case AIState.Patrol:
                agent.isStopped = false;
                SetNewRandomPatrolPoint(); // Devriye başlarken hemen bir nokta belirle
                break;
            case AIState.Chase:
                agent.isStopped = false;
                break;
            case AIState.Attack:
                agent.isStopped = true;
                break;
            case AIState.Search:
                agent.isStopped = false;
                agent.SetDestination(lastSeenPosition); // Son görülen yere git
                searchTimer = 0f; // Arama sayacını sıfırla
                break;
        }
    }

    private bool IsPlayerDetected()
    {

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > visionConeRange)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);

        if (angleToPlayer > visionConeAngle)
            return false;

        if (IsLineOfSightClear(player))
        {
            lastSeenPosition = player.position;
            return true;
        }

        return false;
    }

    private bool IsLineOfSightClear(Transform target)
    {
        // Göz hizasından bir ışın göndererek arada engel olup olmadığını kontrol et
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f; // Göz yüksekliği
        if (Physics.Raycast(eyePosition, (target.position - eyePosition).normalized, out RaycastHit hit, visionConeRange, obstacleLayer | playerLayer))
        {
            // Işın bir şeye çarptı. Eğer çarptığı şey oyuncu ise, görüş açıktır.
            if (hit.transform == target)
            {
                return true;
            }
        }
        return false;
    }

    private void SetNewRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolAreaRadius;
        randomDirection += patrolAreaCenter;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolAreaRadius, NavMesh.AllAreas);
        agent.SetDestination(navHit.position);
        patrolTimer = 0f;
        currentReconsiderDuration = Random.Range(patrolReconsiderTimeRange.x, patrolReconsiderTimeRange.y);
    }

    // --- DEBUGGING ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolAreaCenter, patrolAreaRadius);


        // Görüş konisini çiz (mavi)
        Vector3 fovLine1 = Quaternion.AngleAxis(visionConeAngle / 2, transform.up) * transform.forward * visionConeRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionConeAngle / 2, transform.up) * transform.forward * visionConeRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}