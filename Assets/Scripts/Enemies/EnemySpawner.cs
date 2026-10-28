using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Player Tracking")]
    [Tooltip("The player's position to calculate spawn distances.")]
    [SerializeField] private Transform playerTransform;

    [Header("Wave Settings")]
    [Tooltip("The engine cap: maximum enemies allowed alive at once.")]
    [SerializeField] private int maxActiveEnemies = 24;

    [Tooltip("The wave pool: total enemies that will spawn during this event.")]
    [SerializeField] private int totalEnemiesInWave = 48;

    [Tooltip("Time to wait between individual spawns.")]
    [SerializeField] private float spawnDelay = 3f;

    [Header("Proximity Spawn Logic")]
    [Tooltip("Minimum distance from player to spawn.")]
    [SerializeField] private float minSpawnDistance = 20f;
    [Tooltip("How many of the closest valid spawn points to pick randomly from.")]
    [SerializeField] private int closestSpawnPoolSize = 3;

    private int enemiesSpawnedSoFar = 0;
    public static int ActiveEnemiesCount = 0;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy Prefab is not assigned in the EnemySpawner!");
            return;
        }

        enemiesSpawnedSoFar = 0;
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (enemiesSpawnedSoFar < totalEnemiesInWave)
        {
            if (ActiveEnemiesCount < maxActiveEnemies)
            {
                Transform spawnPoint = GetOptimalSpawnPoint();
                if (spawnPoint != null)
                {
                    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                    ActiveEnemiesCount++;
                    enemiesSpawnedSoFar++;
                }
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Transform GetOptimalSpawnPoint()
    {
        if (spawnPoints.Length == 0 || playerTransform == null) return null;

        var sortedSpawns = spawnPoints
            .OrderBy(sp => Vector3.Distance(playerTransform.position, sp.position))
            .ToList();

        var validSpawns = sortedSpawns
            .Where(sp => Vector3.Distance(playerTransform.position, sp.position) >= minSpawnDistance)
            .ToList();

        if (validSpawns.Count == 0)
        {
            return sortedSpawns.Last();
        }

        int poolSize = Mathf.Min(closestSpawnPoolSize, validSpawns.Count);
        int randomIndex = Random.Range(0, poolSize);

        return validSpawns[randomIndex];
    }
}
