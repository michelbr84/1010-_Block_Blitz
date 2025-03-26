using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene reloading
using UnityEngine.UI; // Needed for UI manipulation if required

/// <summary>
/// The GameController class manages the overall game flow, including initializing the board and spawner,
/// handling user input for shape selection and placement, checking game conditions such as valid shape positions,
/// and triggering the Game Over state with an option to restart the game.
/// It also integrates sound and particle effects via SFXManager and ParticleEffectsManager.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Game Objects")]
    [Tooltip("Reference to the Spawner that creates shapes")]
    [SerializeField] private Spawner spawner;

    [Tooltip("Reference to the game Board")]
    [SerializeField] private Board gameBoard;

    [Header("Camera Settings")]
    [Tooltip("Main camera used for raycasting")]
    [SerializeField] private Camera mainCamera; // Exposed to inspector for easy assignment

    [Header("Input Settings")]
    [Tooltip("Layer mask for selecting shapes")]
    [SerializeField] private LayerMask shapeLayer;

    [Header("Shape Placement Settings")]
    [Tooltip("Offset for the shape position when clicked (applied on top of the mouse position)")]
    [SerializeField] private Vector2 clickPositionOffset;

    [Header("Game Over UI")]
    [Tooltip("Panel that displays the Game Over message and restart option")]
    [SerializeField] private GameObject gameOverUIPanel;

    // The currently selected shape
    private Shape selectedShape;

    // Flag to track if the game is over
    private bool isGameOver = false;

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the game by creating the board and spawning shapes.
    /// </summary>
    void Start()
    {
        // Initialize main camera if not set in inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Ensure the Board reference is set
        if (gameBoard == null)
        {
            gameBoard = Object.FindFirstObjectByType<Board>();
            if (gameBoard == null)
            {
                Debug.LogError("There is no game board defined");
                return;
            }
        }
        gameBoard.CreateBoard();

        // Ensure the Spawner reference is set
        if (spawner == null)
        {
            spawner = Object.FindFirstObjectByType<Spawner>();
            if (spawner == null)
            {
                Debug.LogError("There is no spawner object defined");
                return;
            }
        }
        spawner.SpawnShapes();

        // Hide Game Over UI at start
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Processes shape selection and movement based on user input.
    /// Checks the losing condition each frame.
    /// </summary>
    void Update()
    {
        // Process input only if the game is not over and necessary references are valid.
        if (spawner == null || gameBoard == null || isGameOver)
            return;

        SelectShape();
        MoveSelectedShape();

        // Check the losing condition each frame: if no shape fits on the board, trigger Game Over.
        if (!CheckShapesFitArea())
        {
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Handles selection and placement of shapes using mouse input.
    /// </summary>
    private void SelectShape()
    {
        // Verify if the mouse position is within the screen bounds
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(mousePos);
        RaycastHit2D rayHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, shapeLayer);

        // When the left mouse button is pressed, select a shape if available
        if (Input.GetMouseButtonDown(0))
        {
            if (!rayHit)
                return;

            if (selectedShape == null)
            {
                selectedShape = rayHit.collider.GetComponent<Shape>();
                if (selectedShape != null)
                {
                    // Scale up the shape to indicate selection
                    selectedShape.SetScaleUp();
                }
            }
        }

        // When the left mouse button is released, attempt to place the shape on the board
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedShape == null)
                return;

            // Snap the shape's position to the board grid using the defined offset
            DropShapeOnBoard();

            // Validate if the shape's new position is valid
            if (gameBoard.IsValidPosition(selectedShape))
            {
                // Place the shape on the board
                selectedShape.SetShapeParent(gameBoard.transform);
                selectedShape.SetTagPlaced();
                selectedShape.DestroyCollider();

                gameBoard.StoreShapeInGrid(selectedShape);
                gameBoard.ClearBoard();

                // Play sound effect for placing shape successfully
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayPlaceShape();
                }
                // Play particle effect for placing shape
                if (ParticleEffectsManager.Instance != null)
                {
                    ParticleEffectsManager.Instance.PlayPlaceShapeEffect(selectedShape.transform.position);
                }

                spawner.DecreaseShapeCount();

                // Check if all shapes have been used and spawn new ones if needed
                if (IsShapeCountZero())
                {
                    spawner.SpawnShapes();
                }
            }
            else
            {
                // If the drop position is invalid, reset the shape to its spawn position and scale it down
                selectedShape.SetScaleDown();
                selectedShape.transform.position = selectedShape.GetSpawnPosition();
            }

            // Reset the selected shape
            selectedShape = null;
        }
    }

    /// <summary>
    /// Moves the currently selected shape to follow the mouse position with the defined offset.
    /// </summary>
    private void MoveSelectedShape()
    {
        if (selectedShape == null)
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // Apply the inspector-defined offset so the shape stays relative to the click position
        selectedShape.transform.position = worldPoint + clickPositionOffset;
    }

    /// <summary>
    /// Snaps the selected shape's position to the nearest integer grid cell using the inspector offset.
    /// </summary>
    private void DropShapeOnBoard()
    {
        if (selectedShape == null)
            return;

        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // Snap to nearest grid cell
        Vector3Int snapped = Vector3Int.RoundToInt(worldPoint);
        Vector2 snappedPosition = new Vector2(snapped.x, snapped.y);
        selectedShape.transform.position = snappedPosition + clickPositionOffset;
    }

    /// <summary>
    /// Checks if there are no more shapes left to place.
    /// </summary>
    /// <returns>True if shape count is zero, false otherwise.</returns>
    private bool IsShapeCountZero()
    {
        return (spawner.GetShapeCount() == 0);
    }

    /// <summary>
    /// Checks if the remaining shapes can be placed somewhere on the board.
    /// </summary>
    /// <returns>True if at least one shape can be placed, false otherwise.</returns>
    private bool CheckShapesFitArea()
    {
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("NotPlaced");

        if (shapes.Length > 0)
        {
            foreach (GameObject shapeObj in shapes)
            {
                Debug.Log($"Checking shape: {shapeObj.transform.name}");
                Shape shape = shapeObj.GetComponent<Shape>();
                if (shape != null && !gameBoard.CheckShapeIsFitting(shape))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Triggers the game over sequence when the losing condition is met.
    /// </summary>
    private void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over: No available moves left.");

            // Play Game Over sound effect
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayGameOver();
            }
            // Play Game Over particle effect at the camera's position (or another desired position)
            if (ParticleEffectsManager.Instance != null)
            {
                ParticleEffectsManager.Instance.PlayGameOverEffect(mainCamera.transform.position);
            }

            ShowGameOverUI();
        }
    }

    /// <summary>
    /// Activates the Game Over UI panel.
    /// </summary>
    private void ShowGameOverUI()
    {
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Public method to restart the game. It reloads the current scene.
    /// This method can be linked to a button in the Game Over UI.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
