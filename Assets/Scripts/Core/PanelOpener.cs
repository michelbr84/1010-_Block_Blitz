using UnityEngine;
using UnityEngine.UI; // Necessário para manipular componentes do UI
using System.Collections.Generic; // Necessário para o uso do Dictionary

// This script handles opening a panel, pausing the game, and disabling specified game objects 
// so that the user cannot interact with the game behind the panel.
// Attach this script to any GameObject in your scene.
public class PanelOpener : MonoBehaviour
{
    // Public variable for the button that opens the panel.
    // Drag and drop your Button GameObject here in the Inspector.
    public Button openButton;

    // Public variable for the button that closes the panel.
    // Drag and drop your Button GameObject here in the Inspector.
    public Button closeButton;

    // Public variable for the panel (GameObject) to be opened.
    // Drag and drop the Panel GameObject here in the Inspector.
    public GameObject panelToOpen;

    // Optional: Array of GameObjects to disable when the panel is open.
    // This is useful to prevent user interactions with gameplay objects (e.g., player, enemies, other UI).
    public GameObject[] objectsToDisable;

    // Optional: Dictionary to store the original active state of the objects to disable.
    private Dictionary<GameObject, bool> originalActiveStates = new Dictionary<GameObject, bool>();

    // Start is called before the first frame update.
    void Start()
    {
        // Verify that the openButton is assigned and add the listener for the OpenPanel method.
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenPanel);
        }
        else
        {
            Debug.LogError("Open Button is not assigned in the Inspector");
        }

        // Verify that the closeButton is assigned and add the listener for the ClosePanel method.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogWarning("Close Button is not assigned in the Inspector. Ensure you have an alternative way to close the panel.");
        }

        // Verify that the panelToOpen is assigned and disable it at the start.
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(false);
        }
        else
        {
            Debug.LogError("Panel to open is not assigned in the Inspector");
        }

        // Save the original active state for each object that may be disabled.
        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null)
                {
                    originalActiveStates[obj] = obj.activeSelf;
                }
            }
        }
    }

    // Method called when the open button is clicked.
    void OpenPanel()
    {
        if (panelToOpen != null)
        {
            // Activate the panel.
            panelToOpen.SetActive(true);
            // Pause the game.
            Time.timeScale = 0f;

            // Disable each specified GameObject to prevent interactions with the game behind the panel.
            if (objectsToDisable != null)
            {
                foreach (var obj in objectsToDisable)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Panel to open is not assigned in the Inspector");
        }
    }

    // Method called when the close button is clicked (or can be called by other means).
    public void ClosePanel()
    {
        if (panelToOpen != null)
        {
            // Deactivate the panel.
            panelToOpen.SetActive(false);
            // Resume the game.
            Time.timeScale = 1f;

            // Re-enable the objects that were disabled (restoring their original active state).
            if (objectsToDisable != null)
            {
                foreach (var obj in objectsToDisable)
                {
                    if (obj != null && originalActiveStates.ContainsKey(obj))
                    {
                        obj.SetActive(originalActiveStates[obj]);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Panel to open is not assigned in the Inspector");
        }
    }
}
