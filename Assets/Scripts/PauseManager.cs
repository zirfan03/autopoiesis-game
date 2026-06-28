using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool _isPaused = false;

    private PlayerControlsInputSystem _inputActions;

    void Awake()
    {
        _inputActions = new PlayerControlsInputSystem();
    }

    void OnEnable()
    {
        _inputActions.Menus.Enable();
        _inputActions.Menus.Pause.performed += OnPause;
    }

    void OnDisable()
    {
        _inputActions.Menus.Pause.performed -= OnPause;
        _inputActions.Menus.Disable();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (_isPaused) Resume();
        else Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
