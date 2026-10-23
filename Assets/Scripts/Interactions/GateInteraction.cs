using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GateInteraction : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private TMP_Text gatePromptText;

    [SerializeField] private int gateCost = 500;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private bool isInRange = false;
    private bool hasOpened = false;

    private void Awake()
    {
        if (gatePromptText != null)
            gatePromptText.gameObject.SetActive(false);

        if (gateAnimator != null)
            gateAnimator.enabled = false;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasOpened)
        {
            isInRange = true;

            if (gatePromptText != null)
            {
                gatePromptText.text = "Press 'E' to Open Gate [Cost: " + gateCost + "]";
                gatePromptText.gameObject.SetActive(true);
            }


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
        if (other.CompareTag("Player") && !hasOpened)
        {
            isInRange = false;

            if (gatePromptText != null)
                gatePromptText.gameObject.SetActive(false);

            if (interactAction != null)
            {
                interactAction.performed -= OnInteract;
                interactAction = null;
            }
        }    
    }
   private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInRange && !hasOpened)
        {
            if (ScoreManager.Instance.SpendScore(gateCost))
            {
                OpenGate();
            }
            else
            {
                if (gatePromptText != null)
                {
                    gatePromptText.text = "Not enough Dark Essence!";
                }
            }
        }
    }

    private void OpenGate()
    {
        if (gateAnimator != null)
            gateAnimator.enabled = true;

        gateAnimator.SetTrigger("OpenGate");
        hasOpened = true;

        if (gatePromptText != null)
            gatePromptText.gameObject.SetActive(false);

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

    public void DisableAnimator()
    {
        if (gateAnimator != null)
            gateAnimator.enabled = false;
    }
}
