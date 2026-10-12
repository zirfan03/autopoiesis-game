using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Camera aimCamera;

    [Header("Settings")]
    [SerializeField] private float fireRate = 32f;
    [SerializeField] private bool automatic = true;
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Visual Bullet")]
    [SerializeField] private GameObject visualBulletPrefab;

    [Header("Optional Effects")]
    [SerializeField] private GameObject impactPrefab;

    private bool isFiring;
    private float nextFireTime;

    private void OnEnable()
    {
        fireAction.action.Enable();
        fireAction.action.started += OnFireStarted;
        fireAction.action.canceled += OnFireCanceled;
    }

    private void OnDisable()
    {
        fireAction.action.started -= OnFireStarted;
        fireAction.action.canceled -= OnFireCanceled;
        isFiring = false;
    }

    private void OnFireStarted(InputAction.CallbackContext ctx)
    {
        isFiring = true;
        if (!automatic) TryFire();
    }

    private void OnFireCanceled(InputAction.CallbackContext ctx)
    {
        isFiring = false;
    }

    private void Update()
    {
        if (automatic && isFiring) TryFire();
    }

    private void TryFire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + 1f / fireRate;
        Shoot();
    }

    private void Shoot()
    {
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 endPoint = ray.GetPoint(range);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            IDamageable target =
                hit.collider.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (impactPrefab != null)
            {
                Vector3 position = hit.point + hit.normal * 0.01f;
                Quaternion rotation = Quaternion.LookRotation(-hit.normal);

                GameObject fx = Instantiate(impactPrefab, position, rotation);
                fx.transform.SetParent(hit.collider.transform);
                Destroy(fx, 5f);
            }    
        }

        if (visualBulletPrefab != null && muzzle != null)
        {
            GameObject bulletObj = Instantiate(visualBulletPrefab, muzzle.position, muzzle.rotation);

            if (bulletObj.TryGetComponent(out VisualBullet visualBullet))
            {
                visualBullet.Init(endPoint);
            }
        }
    }
}
