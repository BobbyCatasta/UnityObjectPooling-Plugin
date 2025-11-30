using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example implementation of IPoolable for a cube.
/// Provides basic enable/disable handling and a random direction assignment.
/// </summary>
public class CubeScript : MonoBehaviour, IPoolable
{
    /// <summary>
    /// Pool type used to route this instance back to the correct pool.
    /// </summary>
    [SerializeField] private TypePool typeForPoolIndexing;

    /// <summary>
    /// Movement speed used by this cube (if movement logic is added).
    /// </summary>
    float speed = 0.01f;

    /// <summary>
    /// Movement direction assigned when the cube is enabled.
    /// </summary>
    Vector3 direction;

    /// <summary>
    /// Returns this GameObject to the ObjectPool and deactivates it.
    /// </summary>
    public void DisableObject()
    {
        ObjectPool.Instance.DeactivatePoolObject(gameObject, typeForPoolIndexing);
    }

    /// <summary>
    /// Called when the object is taken from the pool and should be active.
    /// </summary>
    public void EnableObject()
    {
        gameObject.SetActive(true);

        // Assign a random normalized direction for potential movement.
        direction = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized;
    }

    /// <summary>
    /// Example of manual disabling input for testing.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the cube is enabled. 
    /// Currently reserved for potential future logic (e.g., timed disable).
    /// </summary>
    private void OnEnable()
    {
        // StartCoroutine(DisableCoroutine());
    }
}