using UnityEngine;
using UnityEngine.UI;  // For UI Button component

public class ExitGame : MonoBehaviour
{
    [Header("Exit Button")]
    [Tooltip("Drag the UI Button here that will trigger the exit")]
    public Button exitButton;  // Reference to the UI button in the inspector

    // Start is called before the first frame update
    void Start()
    {
        // Check if the exit button has been assigned
        if (exitButton != null)
        {
            // Add a listener to the button click event
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogWarning("Exit Button not assigned in the inspector!");
        }
    }

    // This function is called when the exit button is clicked
    void OnExitButtonClicked()
    {
        // Quit the application
        Application.Quit();
        // Note: Application.Quit() does not work in the Unity Editor.
        Debug.Log("Exiting the game...");
    }
}
