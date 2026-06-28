using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float _currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject deathScreen;

    private bool _isDead = false;

    void Start()
    {
        _currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = _currentHealth;
        }

        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = _currentHealth;
        }

        Debug.Log("Player took damage. Current health: " + _currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (_isDead) return;

        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = _currentHealth;
        }
    }

    void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log("Player has died.");

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Debug.Log("Restart button was clicked.");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Return button was clicked.");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
