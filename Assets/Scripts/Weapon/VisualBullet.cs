using Unity.VisualScripting;
using UnityEngine;

public class VisualBullet : MonoBehaviour
{
    [SerializeField] private float speed = 75f;

    private Vector3 target;
    private bool initialized;

    public void Init(Vector3 targetPoint)
    {
        target = targetPoint;

        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);

        }

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if ((transform.position - target).sqrMagnitude < 0.0001f)
        {
            Destroy(gameObject);
        }
    }
}
