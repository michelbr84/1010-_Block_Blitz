using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Board class is responsible for creating and managing the game board.
/// It handles board creation, validating shape positions, storing shapes in the grid,
/// clearing complete rows/columns, awarding points for each cleared line,
/// and playing sound and particle effects when lines are cleared.
/// </summary>
public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    [Tooltip("Width of the board (number of cells horizontally)")]
    [SerializeField] private int boardWidth = 10;
    [Tooltip("Height of the board (number of cells vertically)")]
    [SerializeField] private int boardHeight = 10;

    [Header("Cell Settings")]
    [Tooltip("Prefab for the empty cell sprite")]
    [SerializeField] private Transform emptyCellSprite;

    [Header("Score Settings")]
    [Tooltip("Points awarded per cleared line (row or column)")]
    [SerializeField] private int pointsPerLine = 10;

    // 2D array representing the grid of cells on the board
    private Transform[,] grid;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the grid based on board dimensions.
    /// </summary>
    private void Awake()
    {
        grid = new Transform[boardWidth, boardHeight];
    }

    /// <summary>
    /// Creates the game board by instantiating empty cell sprites at each grid position.
    /// </summary>
    public void CreateBoard()
    {
        if (emptyCellSprite != null)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Transform cell = Instantiate(emptyCellSprite, new Vector2(x, y), Quaternion.identity);
                    cell.name = $"Cell ({x}, {y})";
                    cell.parent = transform;
                }
            }
        }
        else
        {
            Debug.LogError("Please assign the empty cell sprite in the Inspector.");
        }
    }

    /// <summary>
    /// Checks if the given shape is in a valid position within the board and not colliding with other blocks.
    /// </summary>
    public bool IsValidPosition(Shape shape)
    {
        if (shape == null) return false;

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);

            if (!IsWithinBoard((int)pos.x, (int)pos.y))
                return false;

            if (IsOccupied((int)pos.x, (int)pos.y))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if the coordinates are within the board boundaries.
    /// </summary>
    private bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight);
    }

    /// <summary>
    /// Checks if the cell at the given coordinates is already occupied.
    /// </summary>
    private bool IsOccupied(int x, int y)
    {
        if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            return true;

        return (grid[x, y] != null);
    }

    /// <summary>
    /// Stores the shape's blocks into the grid.
    /// </summary>
    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
            return;

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);
            grid[(int)pos.x, (int)pos.y] = child;
        }
    }

    /// <summary>
    /// Clears complete rows and columns from the board, awards points for each cleared line,
    /// and plays sound and particle effects if any line is cleared.
    /// </summary>
    public void ClearBoard()
    {
        List<int> rowIndex = new List<int>();
        List<int> columnIndex = new List<int>();

        // Check and clear complete rows
        for (int y = 0; y < boardHeight; y++)
        {
            if (IsRowComplete(y))
            {
                ClearRow(y);
                rowIndex.Add(y);
            }
        }

        // Check and clear complete columns
        for (int x = 0; x < boardWidth; x++)
        {
            if (IsColumnComplete(x))
            {
                ClearColumn(x);
                columnIndex.Add(x);
            }
        }

        if (rowIndex.Count > 0)
        {
            ClearRowStack(rowIndex);
        }
        
        if (columnIndex.Count > 0)
        {
            ClearColumnStack(columnIndex);
        }

        // Award points based on the number of lines cleared
        int linesCleared = rowIndex.Count + columnIndex.Count;
        if (linesCleared > 0)
        {
            ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(linesCleared * pointsPerLine);
            }

            // Play sound effect for clearing lines
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayClearLine();
            }

            // Play particle effects for each cleared row
            if (ParticleEffectsManager.Instance != null)
            {
                foreach (int row in rowIndex)
                {
                    // Toca o efeito na posição central da linha limpa
                    ParticleEffectsManager.Instance.PlayClearLineEffect(new Vector3(boardWidth / 2f, row, 0));
                }
                // Play particle effects for each cleared column
                foreach (int col in columnIndex)
                {
                    // Toca o efeito na posição central da coluna limpa
                    ParticleEffectsManager.Instance.PlayClearLineEffect(new Vector3(col, boardHeight / 2f, 0));
                }
            }
        }
    }

    /// <summary>
    /// Determines if a row is completely filled.
    /// </summary>
    private bool IsRowComplete(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            if (grid[x, y] == null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if a column is completely filled.
    /// </summary>
    private bool IsColumnComplete(int x)
    {
        for (int y = 0; y < boardHeight; y++)
        {
            if (grid[x, y] == null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Clears all blocks in the specified row by destroying their GameObjects.
    /// </summary>
    private void ClearRow(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
            }
        }
    }

    /// <summary>
    /// Clears all blocks in the specified column by destroying their GameObjects.
    /// </summary>
    private void ClearColumn(int x)
    {
        for (int y = 0; y < boardHeight; y++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
            }
        }
    }

    /// <summary>
    /// Resets grid references for the cleared rows.
    /// </summary>
    private void ClearRowStack(List<int> rows)
    {
        foreach (int row in rows)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                grid[x, row] = null;
            }
        }
    }

    /// <summary>
    /// Resets grid references for the cleared columns.
    /// </summary>
    private void ClearColumnStack(List<int> columns)
    {
        foreach (int col in columns)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                grid[col, y] = null;
            }
        }
    }

    /// <summary>
    /// Checks if there is any valid position on the board for the given shape.
    /// </summary>
    public bool CheckShapeIsFitting(Shape shape)
    {
        if (shape == null || shape.transform.childCount == 0)
            return false;

        // Use the position of the first child as a reference
        Vector2 firstChildPosition = Vector2Int.RoundToInt(shape.transform.GetChild(0).position);

        for (int y = boardHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (grid[x, y] == null)
                {
                    Vector2 shapePlacePosition = new Vector2(x, y);

                    if (CheckShapeWithSpecificVector(shape, shapePlacePosition, firstChildPosition))
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the shape fits at a specific placement on the board.
    /// </summary>
    private bool CheckShapeWithSpecificVector(Shape shape, Vector2 shapePlacePosition, Vector2 firstChildPosition)
    {
        foreach (Transform child in shape.transform)
        {
            Vector2 childPos = Vector2Int.RoundToInt(child.position);
            Vector2 roundedPos = Vector2Int.RoundToInt(shapePlacePosition + childPos - firstChildPosition);

            if (IsOccupied((int)roundedPos.x, (int)roundedPos.y) || !IsWithinBoard((int)roundedPos.x, (int)roundedPos.y))
                return false;
        }

        return true;
    }
}
