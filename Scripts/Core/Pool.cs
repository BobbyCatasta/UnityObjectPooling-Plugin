using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single pool configuration and runtime state
/// for a specific prefab (queue of instantiated GameObjects).
/// </summary>
public class Pool
{
    /// <summary>
    /// Prefab used to populate this pool.
    /// </summary>
    public GameObject prefabToSpawn;

    /// <summary>
    /// Queue containing all instantiated objects belonging to this pool.
    /// </summary>
    public Queue<GameObject> instantiatedGameObjects;

    /// <summary>
    /// Initial number of instances to create for this pool.
    /// </summary>
    public int quantity;

    /// <summary>
    /// Logical type used to categorize this pool.
    /// </summary>
    public TypePool typePool;

    /// <summary>
    /// Creates a new pool with the given prefab, quantity, and pool type.
    /// </summary>
    /// <param name="prefabToSpawn">Prefab used to populate the pool.</param>
    /// <param name="quantity">Number of instances to create.</param>
    /// <param name="typePool">Logical type used to categorize the pool.</param>
    public Pool(GameObject prefabToSpawn, int quantity, TypePool typePool)
    {
        this.prefabToSpawn = prefabToSpawn;
        this.quantity = quantity;
        instantiatedGameObjects = new Queue<GameObject>();
        this.typePool = typePool;
    }
}

/// <summary>
/// Interface that defines the contract for poolable objects.
/// Implementors decide what happens when the object is enabled/disabled.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when an object is taken from the pool and should be enabled.
    /// </summary>
    public void EnableObject();

    /// <summary>
    /// Called when an object should be returned to the pool and disabled.
    /// </summary>
    public void DisableObject();
}

/// <summary>
/// Simple enum used to categorize different pool types.
/// </summary>
public enum TypePool
{
    CUBE,
    SPHERE
}