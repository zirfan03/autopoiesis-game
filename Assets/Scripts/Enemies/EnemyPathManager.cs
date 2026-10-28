using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance { get; private set; }

    [Header("Optimization Settings")]
    [Tooltip("Maximum number of NavMesh paths to calculate per frame.")]
    [SerializeField] private int maxPathUpdatesPerFrame = 5;

    private Queue<zombieEnemyAI> zombieQueue = new Queue<zombieEnemyAI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (zombieQueue.Count == 0) return;

        int updatesThisFrame = Mathf.Min(maxPathUpdatesPerFrame, zombieQueue.Count);

        for (int i = 0; i < updatesThisFrame; i++)
        {
            zombieEnemyAI currentZombie = zombieQueue.Dequeue();

            if (currentZombie != null && currentZombie.IsAlive)
            {
                currentZombie.UpdateNavMeshPath();
                zombieQueue.Enqueue(currentZombie);
            }
        }
    }

    public void RegisterZombie(zombieEnemyAI zombie)
    {
        if (!zombieQueue.Contains(zombie))
        {
            zombieQueue.Enqueue(zombie);
        }
    }
}
