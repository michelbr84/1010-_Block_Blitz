using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Spawner class is responsible for instantiating shapes at designated spawn points.
/// It handles spawning a set number of shapes, tracking the count of available shapes,
/// and providing functionality to decrease the count when a shape is used.
/// It also integrates sound effects via the SFXManager when new shapes are spawned.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("List of shape prefabs that can be spawned")]
    [SerializeField] private Shape[] shapes;

    [Tooltip("List of spawn point transforms where shapes will be instantiated")]
    [SerializeField] private Transform[] spawnPoints;

    // Tracks the current number of shapes available in stock
    private int currentShapeInStock = 0;

    /// <summary>
    /// Spawns shapes at the defined spawn points.
    /// Each spawned shape is set to a scaled-down version, parented to its spawn point,
    /// and its spawn position is recorded.
    /// After spawning, a sound effect is played via SFXManager.
    /// </summary>
    public void SpawnShapes()
    {
        // Loop through each spawn point and instantiate a shape
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Instantiate a random shape at the current spawn point's position with default rotation
            Shape shape = Instantiate(GetRandomShape(), spawnPoints[i].position, Quaternion.identity);
            
            // Set the shape to its initial scaled-down size
            shape.SetScaleDown();
            
            // Parent the shape to the spawn point for organization
            shape.SetShapeParent(spawnPoints[i]);
            
            // Record the spawn position for later resets if needed
            shape.SetSpawnPosition(spawnPoints[i].position);
        }

        // Update the count of available shapes (here assuming one per spawn point)
        currentShapeInStock = spawnPoints.Length;
        
        // Play sound effect for spawning shapes
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySpawnShape();
        }
    }

    /// <summary>
    /// Returns a random shape from the available shapes array.
    /// </summary>
    /// <returns>A randomly selected shape prefab.</returns>
    private Shape GetRandomShape()
    {
        return shapes[Random.Range(0, shapes.Length)];
    }

    /// <summary>
    /// Decreases the count of shapes in stock.
    /// </summary>
    public void DecreaseShapeCount()
    {
        if (currentShapeInStock > 0)
        {
            currentShapeInStock--;
        }
    }

    /// <summary>
    /// Gets the current count of shapes available in stock.
    /// </summary>
    /// <returns>The number of shapes in stock.</returns>
    public int GetShapeCount()
    {
        return currentShapeInStock;
    }
}
