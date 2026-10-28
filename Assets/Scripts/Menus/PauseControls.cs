using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControls : MonoBehaviour
{
    public static PauseControls Instance { get; private set; }
    public static event Action<bool> OnGamePaused;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    public bool IsPaused { get; private set; } = false;

    private PlayerControlsInputSystem _inputActions;

    private float _cachedTimeScale = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _inputActions = new PlayerControlsInputSystem();
    }

    void OnEnable()
    {
        _inputActions.Menus.Enable();
        _inputActions.Menus.Pause.performed += OnPauseInput;
    }

    void OnDisable()
    {
        _inputActions.Menus.Pause.performed -= OnPauseInput;
        _inputActions.Menus.Disable();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    private void OnPauseInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        _cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnGamePaused?.Invoke(true);
    }
    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;

        Time.timeScale = _cachedTimeScale;
        AudioListener.pause = false;

        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnGamePaused?.Invoke(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(0);
    }
}
