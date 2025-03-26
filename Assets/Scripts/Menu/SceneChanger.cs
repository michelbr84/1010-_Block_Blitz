using UnityEngine;
using UnityEngine.UI;               // For Button component
using UnityEngine.SceneManagement;  // For scene management

public class SceneChanger : MonoBehaviour
{
    [Header("Button to trigger scene change")]
    [Tooltip("Drag the UI Button here")]
    public Button myButton;         // Reference to the UI button in the inspector

    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when the button is clicked (default is 'Game')")]
    public string sceneName = "Game";   // Name of the scene to load, default is "Game"

    // Start is called before the first frame update
    void Start()
    {
        // Check if the button reference is assigned
        if(myButton != null)
        {
            // Add listener to the button click event
            myButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning("Button not assigned in the inspector!");
        }
    }

    // This function is called when the button is clicked
    void OnButtonClicked()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}
