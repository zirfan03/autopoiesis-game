using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    public int startingScore = 9999;
    public int currentScore = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentScore = startingScore;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    public bool SpendScore(int amount)
    {
        if (currentScore >= amount)
        {
            currentScore -= amount;
            UpdateScoreUI();
            return true;
        }

        return false;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Dark Essence: " + currentScore.ToString();
        }
    }
}
