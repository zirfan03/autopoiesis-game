using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpawnButton : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private bool spawnOnlyOnce = true;

    private bool hasSpawned = false;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerTeleportation player = other.GetComponentInParent<PlayerTeleportation>();

        if (player != null)
            player.EnterEnemySpawnButton(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerTeleportation player = other.GetComponentInParent<PlayerTeleportation>();

        if (player != null)
            player.ExitEnemySpawnButton(this);
    }

    public void SpawnEnemy()
    {
        if (spawnOnlyOnce && hasSpawned)
            return;

        if (enemyPrefab == null || enemySpawnPoint == null)
        {
            Debug.LogWarning("EnemySpawnButton is missing references.");
            return;
        }

        Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
        hasSpawned = true;
    }
}