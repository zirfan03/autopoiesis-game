using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private GameObject magazineObject;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoDisplay;

    [Header("Ammo Settings")]
    [SerializeField] private int magSize = 30;
    [SerializeField] private int maxReserveAmmo = 240;
    [SerializeField] private float reloadTime = 4f;

    [Header("Settings")]
    [SerializeField] private float fireRate = 16f;
    [SerializeField] private bool automatic = true;
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Visual Bullet")]
    [SerializeField] private GameObject visualBulletPrefab;

    [Header("Optional Effects")]
    [SerializeField] private GameObject impactPrefab;

    private bool isFiring;
    private bool isSprinting;
    private float nextFireTime;

    private int currentAmmo;
    private int reserveAmmo;
    private bool isReloading;
    private Coroutine activeReloadCoroutine;

    private void Start()
    {
        currentAmmo = magSize;
        reserveAmmo = maxReserveAmmo;
        UpdateAmmoUI();
    }

    private void OnEnable()
    {
        fireAction.action.Enable();
        fireAction.action.started += OnFireStarted;
        fireAction.action.canceled += OnFireCanceled;

        if (reloadAction != null)
        {
            reloadAction.action.Enable();
            reloadAction.action.performed += OnReloadPerformed;
        }

        if (sprintAction != null)
        {
            sprintAction.action.Enable();
            sprintAction.action.started += OnSprintStarted;
            sprintAction.action.canceled += OnSprintCanceled;
        }
    }

    private void OnDisable()
    {
        fireAction.action.started -= OnFireStarted;
        fireAction.action.canceled -= OnFireCanceled;
        isFiring = false;

        if (reloadAction != null) reloadAction.action.performed -= OnReloadPerformed;

        if (sprintAction != null)
        {
            sprintAction.action.started -= OnSprintStarted;
            sprintAction.action.canceled -= OnSprintCanceled;
            isSprinting = false;
        }
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

    private void OnReloadPerformed(InputAction.CallbackContext ctx)
    {
        TryStartReload();
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        isSprinting = true;

        if (isReloading)
        {
            CancelReload();
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
    }

    private void Update()
    {
        if (automatic && isFiring) TryFire();
    }

    private void TryFire()
    {
        if (isReloading || isSprinting) return;

        if (currentAmmo <= 0)
        {
            TryStartReload();
            return;
        }

        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + 1f / fireRate;
        Shoot();
    }

    private void TryStartReload()
    {
        if (isSprinting) return;

        if (!isReloading && currentAmmo < magSize && reserveAmmo > 0)
        {
            activeReloadCoroutine = StartCoroutine(ReloadRoutine());
        }
    }

    private void CancelReload()
    {
        if (activeReloadCoroutine != null)
        {
            StopCoroutine(activeReloadCoroutine);
            activeReloadCoroutine = null;
        }

        isReloading = false;

        if (magazineObject != null) magazineObject.SetActive(true);
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

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

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (magazineObject != null) magazineObject.SetActive(false);

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        if (magazineObject != null) magazineObject.SetActive(true);
        UpdateAmmoUI();

        isReloading = false;
        activeReloadCoroutine = null;
    }

    public void RefillReserveAmmo()
    {
        reserveAmmo = maxReserveAmmo;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = $"{currentAmmo} / {reserveAmmo}";
        }
    }
}
