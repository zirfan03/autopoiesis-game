using UnityEngine;
using UnityEngine.InputSystem;

public class AmmoStation : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("UI")]
    [SerializeField] private GameObject promptUI;

    private GameObject playerInZone;

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
        if (other.GetComponentInChildren<WeaponFire>() != null)
        {
            playerInZone = other.gameObject;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInZone != null && other.gameObject == playerInZone)
        {
            playerInZone = null;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerInZone != null)
        {
            WeaponFire activeWeapon = playerInZone.GetComponentInChildren<WeaponFire>();

            if (activeWeapon != null)
            {
                activeWeapon.RefillReserveAmmo();
            }
        }
    }
}
