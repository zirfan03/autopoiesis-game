using UnityEngine;

public class TestRangedEnemyAI : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectile;
    public Transform firePoint;

    public float fireRate = 5f;
    public float projectileSpeed = 15f;

    [Header("Targeting")]
    public Transform player;
    public float detectionRange = 100f;

    private float _fireTimer;


    void Update()
    {
        _fireTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (_fireTimer >= fireRate && distanceToPlayer <= detectionRange)
        {
            Shoot();
            _fireTimer = 0f;
        }
    }

    void Shoot()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity);
        RangedProjectile firedProjectile = projectileObj.GetComponent<RangedProjectile>();

        firedProjectile.speed = projectileSpeed;
        firedProjectile.Init(direction);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
