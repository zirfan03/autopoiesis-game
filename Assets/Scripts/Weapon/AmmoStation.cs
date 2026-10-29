using UnityEngine;
using UnityEngine.InputSystem;

public class AmmoStation : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("UI")]
    [SerializeField] private GameObject promptUI;

    private WeaponFire playerWeaponInRange;

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponFire weapon = other.GetComponentInChildren<WeaponFire>();
        if (weapon != null)
        {
            playerWeaponInRange = weapon;

            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeaponFire weapon = other.GetComponentInChildren<WeaponFire>();
        if (weapon != null && weapon == playerWeaponInRange)
        {
            playerWeaponInRange = null;

            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerWeaponInRange != null)
        {
            playerWeaponInRange.RefillReserveAmmo();
        }
    }
}
