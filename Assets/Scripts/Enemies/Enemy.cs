using UnityEngine;

[RequireComponent(typeof(zombieEnemyAI))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float healthScalePerWave = 1.15f;
    [SerializeField] private int pointsOnKill = 150;

    private float currentHealth;
    private zombieEnemyAI zombieAI;
    private bool isDead = false;

    private void Awake()
    {
        zombieAI = GetComponent<zombieEnemyAI>();
    }

    public void InitializeHealth(int waveNumber)
    {
        currentHealth = baseHealth * Mathf.Pow(healthScalePerWave, waveNumber - 1);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            isDead = true;

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsOnKill);
            }

            zombieAI.TriggerDeath();

            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.OnEnemyDefeated();
            }
        }
    }
}
