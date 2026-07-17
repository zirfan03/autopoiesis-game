using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestMeleeEnemyAI : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float turnSlowdownMultiplier = 0.25f;
    [SerializeField] private float updateRate = 0.05f;

    private NavMeshAgent agent;
    private Transform player;
    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 720f;
        agent.acceleration = 10f;
        agent.stoppingDistance = 10f;
        agent.autoBraking = false;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            agent.SetDestination(player.position);
            timer = updateRate;
        }

        Vector3 toCorner = agent.steeringTarget - transform.position;
        toCorner.y = 0f;

        if (toCorner.sqrMagnitude > 0.01f)
        {
            float angle = Vector3.Angle(transform.forward, toCorner.normalized);
            float t = angle / 180f;
            float speedMultipler = Mathf.Lerp(1f, turnSlowdownMultiplier, t);
            agent.speed = normalSpeed * speedMultipler;
        }
        else
        {
            agent.speed = normalSpeed;
        }
    }
}