using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponSway weaponSway;

    [Header("Input")]
    [SerializeField] private InputActionReference swapAction;

    private int currentWeaponIndex = 0;

    private void OnEnable()
    {
        {
            swapAction.action.Enable();
            swapAction.action.performed += OnWeaponSwap;
        }
    }

    private void OnDisable()
    {
        if (swapAction != null)
        {
            swapAction.action.performed -= OnWeaponSwap;
        }
    }

    private void Start()
    {
        SelectWeapon(currentWeaponIndex);
    }

    private void OnWeaponSwap(InputAction.CallbackContext ctx)
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponHolder.childCount)
        {
            currentWeaponIndex = 0;
        }
        SelectWeapon(currentWeaponIndex);
    }

    private void SelectWeapon(int index)
    {
        int i = 0;
        foreach (Transform weapon in weaponHolder)
        {
            if (i == index)
            {
                weapon.gameObject.SetActive(true);

                if (weaponSway != null)
                {
                    weaponSway.weaponAim = weapon.GetComponent<WeaponAim>();
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
