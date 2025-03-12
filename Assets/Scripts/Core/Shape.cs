using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Shape class handles the properties and behaviors of a shape object.
/// It stores the spawn position, manages scale changes, parent assignment,
/// collider destruction, and tag setting after the shape is placed.
/// </summary>
public class Shape : MonoBehaviour
{
    [Header("Shape Settings")]
    [Tooltip("The spawn position for this shape")]
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    /// <summary>
    /// Sets the spawn position of the shape.
    /// </summary>
    /// <param name="value">The new spawn position.</param>
    public void SetSpawnPosition(Vector3 value)
    {
        spawnPosition = value;
    }

    /// <summary>
    /// Gets the spawn position of the shape.
    /// </summary>
    /// <returns>The current spawn position.</returns>
    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

    /// <summary>
    /// Sets the parent transform of the shape.
    /// </summary>
    /// <param name="parent">The new parent transform.</param>
    public void SetShapeParent(Transform parent)
    {
        transform.parent = parent;
    }

    /// <summary>
    /// Scales the shape down to a smaller size.
    /// </summary>
    public void SetScaleDown()
    {
        // Adjust the local scale to 55% for X and Y while keeping Z at 1
        transform.localScale = new Vector3(0.55f, 0.55f, 1f);
    }

    /// <summary>
    /// Scales the shape up to its original size using linear interpolation.
    /// </summary>
    public void SetScaleUp()
    {
        // Lerp from the current scale to full scale (1, 1, 1) instantly
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 1f);
    }

    /// <summary>
    /// Destroys the BoxCollider2D component attached to the shape.
    /// </summary>
    public void DestroyCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }

    /// <summary>
    /// Sets the tag of the shape's game object to "Placed".
    /// </summary>
    public void SetTagPlaced()
    {
        gameObject.tag = "Placed";
    }
}
