using UnityEngine;

[RequireComponent(typeof(zombieEnemyAI))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;

    [SerializeField] private int pointsOnKill = 100;

    private zombieEnemyAI zombieAI;
    private bool isDead = false;

    private void Awake()
    {
        zombieAI = GetComponent<zombieEnemyAI>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0f)
        {
            isDead = true;

            ScoreManager.Instance.AddScore(pointsOnKill);

            zombieAI.TriggerDeath();

            EnemySpawner.ActiveEnemiesCount--;
        }
    }
}
