using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class zombieEnemyAI : MonoBehaviour
{
    private enum ZombieState {  Spawning, Chasing, Attacking, Dead }
    private ZombieState currentState = ZombieState.Spawning;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float turnSlowdownMultiplier = 0.25f;
    [SerializeField] private bool isRunning = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 2f;

    [SerializeField] private float walkWindUpDelay = 1f;
    [SerializeField] private float runWindUpDelay = 0.25f;

    [Header("Swarm Settings")]
    [Tooltip("How far around the player should the zombies try to swarm.")]
    [SerializeField] private float swarmRadius = 1.5f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer meshRenderer;

    private NavMeshAgent agent;
    private Transform player;
    private IDamageable playerDamageable;

    private float attackTimer;
    private Color originalColor;

    public bool IsAlive => currentState != ZombieState.Dead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 720f;
        agent.acceleration = 10f;
        agent.stoppingDistance = attackRange;
        agent.autoBraking = true;

        agent.avoidancePriority = Random.Range(10, 90);

        if (meshRenderer != null)
            originalColor = meshRenderer.material.color;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerDamageable = playerObj.GetComponent<IDamageable>();
        }

        Invoke(nameof(FinishSpawning), 2f);
    }

    private void Update()
    {
        if (currentState == ZombieState.Dead || currentState == ZombieState.Spawning || player == null)
            return;

        UpdateAnimator();

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        Vector3 flatZombiePos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatPlayerPos = new Vector3(player.position.x, 0f, player.position.z);
        float distanceToPlayer = Vector3.Distance(flatZombiePos, flatPlayerPos);

        switch (currentState)
        {
            case ZombieState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case ZombieState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
        }
    }

    private void HandleChasing(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange && attackTimer <= 0f)
        {
            currentState = ZombieState.Attacking;
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                attackTimer = isRunning ? runWindUpDelay : walkWindUpDelay;
            }
            return;
        }

        ApplyTurnSlowdown();
    }

    public void UpdateNavMeshPath()
    {
        if (currentState == ZombieState.Dead || currentState == ZombieState.Spawning || player == null)
            return;

        if (currentState == ZombieState.Chasing && agent.isOnNavMesh)
        {
            Vector2 randomOffset = Random.insideUnitCircle * swarmRadius;
            Vector3 swarmTarget = player.position + new Vector3(randomOffset.x, 0f, randomOffset.y);

            agent.SetDestination(player.position);
        }
    }

    private void HandleAttacking(float distanceToPlayer)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        if (attackTimer <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            attackTimer = attackCooldown;
            StartCoroutine(VisualAttackLunge());

            if (distanceToPlayer <= (attackRange + 0.5f) && playerDamageable != null)
            {
                playerDamageable.TakeDamage(attackDamage);
            }
        }

        if (distanceToPlayer > attackRange)
        {
            currentState = ZombieState.Chasing;
            agent.isStopped = false;
        }
    }

    private IEnumerator VisualAttackLunge()
    {
        if (meshRenderer != null) meshRenderer.material.color = Color.red;
        transform.position += transform.forward * 0.25f;

        yield return new WaitForSeconds(0.2f);

        if (meshRenderer != null) meshRenderer.material.color = originalColor;
    }

    private void ApplyTurnSlowdown()
    {
        Vector3 toCorner = agent.steeringTarget - transform.position;
        toCorner.y = 0f;

        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (toCorner.sqrMagnitude > 0.01f)
        {
            float angle = Vector3.Angle(transform.forward, toCorner.normalized);
            float t = angle / 180f;
            float speedMultiplier = Mathf.Lerp(1f, turnSlowdownMultiplier, t);
            agent.speed = targetSpeed * speedMultiplier;
        }
        else
        {
            agent.speed = targetSpeed;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void FinishSpawning()
    {
        currentState = ZombieState.Chasing;
        agent.enabled = true;

        if (EnemyPathManager.Instance != null)
        {
            EnemyPathManager.Instance.RegisterZombie(this);
        }
    }

    public void TriggerDeath()
    {
        currentState = ZombieState.Dead;
        agent.isStopped = true;
        agent.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        if (meshRenderer != null) meshRenderer.material.color = Color.black;
        transform.localScale = new Vector3(1f, 0.2f, 1f);
        Destroy(gameObject, 2f);
    }
}
