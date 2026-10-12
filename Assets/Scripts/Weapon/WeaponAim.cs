using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [Header("Configuration")]
    public bool enableAiming = true;

    [Header("Input")]
    [SerializeField] private InputActionReference aimAction;

    [Header("References")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera weaponCamera;
    [SerializeField] private GameObject crosshair;

    [Header("Hip and Aim Positions (local to camera)")]
    [SerializeField] private Vector3 hipPosition = new Vector3(0.3125f, -0.325f, 1.5f);
    [SerializeField] private Vector3 aimPosition = new Vector3(0f, -0.20625f, 1.625f);

    [Header("Field of View")]
    [SerializeField] private float hipFOV = 60f;
    [SerializeField] private float aimFOV = 40f;

    [Header("Speed")]
    [SerializeField] private float aimSpeed = 15f;

    public bool IsAiming { get; private set; }

    private void OnEnable()
    {
        aimAction.action.Enable();
        aimAction.action.started += OnAimStarted;
        aimAction.action.canceled += OnAimCanceled;
    }

    private void OnDisable()
    {
        aimAction.action.started -= OnAimStarted;
        aimAction.action.canceled -= OnAimCanceled;
        IsAiming = false;
    }

    private void OnAimStarted(InputAction.CallbackContext ctx)
    {
        if (!enableAiming) return;

        IsAiming = true;
        if (crosshair != null) crosshair.SetActive(false);
    }

    private void OnAimCanceled(InputAction.CallbackContext ctx)
    {
        IsAiming = false;
        if (crosshair != null) crosshair.SetActive(true);
    }

    private void Update()
    {
        if (!enableAiming && IsAiming)
        {
            IsAiming = false;
            if (crosshair != null) crosshair.SetActive(true);
        }

        float t = aimSpeed * Time.deltaTime;

        Vector3 targetPos = IsAiming ? aimPosition : hipPosition;
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPos, t);

        float targetFOV = IsAiming ? aimFOV : hipFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, t);

        if (weaponCamera != null)
        {
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, targetFOV, t);
        }
    }
}
