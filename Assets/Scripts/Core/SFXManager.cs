using UnityEngine;

/// <summary>
/// The SFXManager class handles playing sound effects in the game.
/// It uses the Singleton pattern so that other scripts can easily call its methods
/// to play sounds when specific game events occur (e.g., clearing lines, placing shapes, game over).
/// </summary>
public class SFXManager : MonoBehaviour
{
    // Singleton instance
    public static SFXManager Instance { get; private set; }

    [Header("Audio Clips")]
    [Tooltip("Sound effect for clearing a row or column")]
    [SerializeField] private AudioClip clearLineSFX;
    [Tooltip("Sound effect for successfully placing a shape")]
    [SerializeField] private AudioClip placeShapeSFX;
    [Tooltip("Sound effect for game over")]
    [SerializeField] private AudioClip gameOverSFX;
    [Tooltip("Sound effect for spawning shapes")]
    [SerializeField] private AudioClip spawnShapeSFX;

    // AudioSource component used to play the sound effects
    private AudioSource audioSource;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Try to get an existing AudioSource, otherwise add one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Plays the sound effect for clearing a line.
    /// </summary>
    public void PlayClearLine()
    {
        if (clearLineSFX != null)
        {
            audioSource.PlayOneShot(clearLineSFX);
        }
    }

    /// <summary>
    /// Plays the sound effect for placing a shape.
    /// </summary>
    public void PlayPlaceShape()
    {
        if (placeShapeSFX != null)
        {
            audioSource.PlayOneShot(placeShapeSFX);
        }
    }

    /// <summary>
    /// Plays the sound effect for game over.
    /// </summary>
    public void PlayGameOver()
    {
        if (gameOverSFX != null)
        {
            audioSource.PlayOneShot(gameOverSFX);
        }
    }

    /// <summary>
    /// Plays the sound effect for spawning shapes.
    /// </summary>
    public void PlaySpawnShape()
    {
        if (spawnShapeSFX != null)
        {
            audioSource.PlayOneShot(spawnShapeSFX);
        }
    }
}
