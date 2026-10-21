using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy Prefab is not assigned in the EnemySpawner!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
