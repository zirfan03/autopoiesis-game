using UnityEngine;

public class RangedProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 20f;
    public float damage = 30f;

    private Vector3 _direction;

    public void Init(Vector3 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with the enemy itself
        if (other.CompareTag("Enemy")) return;

        // TODO: deal damage if other.CompareTag("Player")
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
