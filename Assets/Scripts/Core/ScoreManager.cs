using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The ScoreManager class handles the player's score, updating the UI,
/// and providing methods to add or reset the score.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Text element used to display the score")]
    [SerializeField] private Text scoreText;

    [Header("Score Settings")]
    [Tooltip("Initial score value")]
    [SerializeField] private int initialScore = 0;

    // Internal score value
    private int score;

    /// <summary>
    /// Initializes the score.
    /// </summary>
    private void Start()
    {
        ResetScore();
    }

    /// <summary>
    /// Adds the specified amount to the current score and updates the UI.
    /// </summary>
    /// <param name="amount">Amount to add to the score</param>
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    /// <summary>
    /// Resets the score to the initial value and updates the UI.
    /// </summary>
    public void ResetScore()
    {
        score = initialScore;
        UpdateScoreUI();
    }

    /// <summary>
    /// Updates the score display on the UI.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogWarning("Score Text UI element is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Returns the current score.
    /// </summary>
    public int GetScore()
    {
        return score;
    }
}
