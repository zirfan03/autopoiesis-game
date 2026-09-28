
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerkInteraction : MonoBehaviour
{
    [Header("Perk Settings")]
    [Tooltip("Give all instances of perk the same ID")]
    [SerializeField] private string perkID;

    [SerializeField] private TMP_Text perkPromptText;
    [SerializeField] private UnityEngine.UI.Image perkIcon;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private bool isInRange = false;

    private static HashSet<string> purchasedPerks = new HashSet<string>();

    private bool hasBought => purchasedPerks.Contains(perkID);

    private void Awake()
    {
        if (perkPromptText != null)
            perkPromptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBought)
        {
            isInRange = true;

            if (perkPromptText != null)
                perkPromptText.gameObject.SetActive(true);

            playerInput = other.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                interactAction = playerInput.actions.FindAction("Interact");

                if (interactAction != null)
                    interactAction.performed += OnInteract;
                else
                    Debug.LogError("Could not find an action named 'Interact'.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasBought)
        {
            isInRange = false;

            if (perkPromptText != null)
                perkPromptText.gameObject.SetActive(false);

            if (interactAction != null)
            {
                interactAction.performed -= OnInteract;
                interactAction = null;
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInRange && !hasBought)
        {
            BuyPerk();
        }
    }

    private void BuyPerk()
    {
        purchasedPerks.Add(perkID);

        if (perkPromptText != null)
            perkPromptText.gameObject.SetActive(false);

        if (perkIcon != null)
            perkIcon.gameObject.SetActive(true);

        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
            interactAction = null;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }

    public static void ResetAllPerks()
    {
        purchasedPerks.Clear();
    }
}
