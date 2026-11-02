using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public static int ActiveEnemiesCount = 0;

    [Header("Player Tracking")]
    [SerializeField] private Transform playerTransform;

    [Header("Proximity Spawn Logic")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float minSpawnDistance = 20f;
    [SerializeField] private int closestSpawnPoolSize = 3;

    [Header("Wave & UI Settings")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnDelay = 1.5f;
    [SerializeField] private int maxActiveEnemies = 24;
    [SerializeField] private TMP_Text waveText;

    [Header("Normal Zombies")]
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private int baseNormalEnemies = 10;
    [SerializeField] private float normalCountMultiplier = 1.25f;

    [Header("Sprinter Zombies")]
    [SerializeField] private GameObject sprinterPrefab;
    [SerializeField] private int sprinterStartWave = 5;
    [SerializeField] private int sprinterBaseCount = 2;
    [SerializeField] private int sprinterIncrement = 2;
    [SerializeField] private int sprinterInterval = 3;

    public int CurrentWave { get; private set; } = 0;
    private int totalEnemiesToSpawn = 0;
    private int sprintersToSpawn = 0;

    private bool wavesHaveStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    public void SpawnEnemies()
    {
        if (wavesHaveStarted) return;

        wavesHaveStarted = true;
        StartCoroutine(NextWaveRoutine());
    }

    private IEnumerator NextWaveRoutine()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        CurrentWave++;
        if (waveText != null) waveText.text = $"WAVE {CurrentWave}";

        CalculateWaveStats();
        StartCoroutine(SpawnWaveRoutine());
    }

    private void CalculateWaveStats()
    {
        int normalCount = Mathf.FloorToInt(baseNormalEnemies * Mathf.Pow(normalCountMultiplier, CurrentWave - 1));

        sprintersToSpawn = 0;
        if (CurrentWave >= sprinterStartWave)
        {
            int intervalsPassed = (CurrentWave - sprinterStartWave) / sprinterInterval;
            sprintersToSpawn = sprinterBaseCount + (intervalsPassed * sprinterIncrement);
        }

        totalEnemiesToSpawn = normalCount + sprintersToSpawn;
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (totalEnemiesToSpawn > 0)
        {
            if (ActiveEnemiesCount < maxActiveEnemies)
            {
                Transform spawnPoint = GetOptimalSpawnPoint();
                if (spawnPoint != null)
                {
                    SpawnEnemy(spawnPoint);
                }
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        GameObject prefabToSpawn = normalPrefab;

        bool spawnSprinter = sprintersToSpawn > 0 && (Random.value > 0.7f || totalEnemiesToSpawn <= sprintersToSpawn);
        if (spawnSprinter)
        {
            prefabToSpawn = sprinterPrefab;
            sprintersToSpawn--;
        }

        GameObject enemyObj = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        if (enemyObj.TryGetComponent(out Enemy enemyScript))
        {
            enemyScript.InitializeHealth(CurrentWave);
        }

        ActiveEnemiesCount++;
        totalEnemiesToSpawn--;
    }

    public void OnEnemyDefeated()
    {
        ActiveEnemiesCount--;

        if (ActiveEnemiesCount <= 0 && totalEnemiesToSpawn <= 0)
        {
            StartCoroutine(NextWaveRoutine());
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
