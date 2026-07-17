using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleportation : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text teleportPromptText;
    [SerializeField] private TMP_Text enemySpawnPromptText;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private TeleporterPad currentPad;
    private EnemySpawnButton currentEnemyButton;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (teleportPromptText != null)
            teleportPromptText.gameObject.SetActive(false);

        if (enemySpawnPromptText != null)
            enemySpawnPromptText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        interactAction = playerInput.actions.FindAction("Interact");

        if (interactAction != null)
            interactAction.performed += OnInteract;
        else
            Debug.LogError("Could not find an action named 'Interact'.");
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentPad != null)
        {
            currentPad.Teleport();
            return;
        }

        if (currentEnemyButton != null)
        {
            currentEnemyButton.SpawnEnemy();
        }
    }

    public void EnterPad(TeleporterPad pad)
    {
        currentPad = pad;
        currentEnemyButton = null;

        if (teleportPromptText != null)
            teleportPromptText.gameObject.SetActive(true);

        if (enemySpawnPromptText != null)
            enemySpawnPromptText.gameObject.SetActive(false);
    }

    public void ExitPad(TeleporterPad pad)
    {
        if (currentPad != pad) return;

        currentPad = null;

        if (teleportPromptText != null)
            teleportPromptText.gameObject.SetActive(false);
    }

    public void EnterEnemySpawnButton(EnemySpawnButton enemyButton)
    {
        currentEnemyButton = enemyButton;
        currentPad = null;

        if (enemySpawnPromptText != null)
            enemySpawnPromptText.gameObject.SetActive(true);

        if (teleportPromptText != null)
            teleportPromptText.gameObject.SetActive(false);
    }

    public void ExitEnemySpawnButton(EnemySpawnButton enemyButton)
    {
        if (currentEnemyButton != enemyButton) return;

        currentEnemyButton = null;

        if (enemySpawnPromptText != null)
            enemySpawnPromptText.gameObject.SetActive(false);
    }
}
