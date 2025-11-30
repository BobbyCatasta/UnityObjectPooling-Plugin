using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central pool manager that uses a ScriptablePool to instantiate and manage
/// multiple prefab-based pools at runtime.
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    /// <summary>
    /// Scriptable configuration that defines which objects to pool.
    /// </summary>
    [SerializeField] private ScriptablePool scriptablePool;

    /// <summary>
    /// Internal list of pools created from the ScriptablePool configuration.
    /// </summary>
    private List<Pool> pools = new List<Pool>();

    /// <summary>
    /// Initializes the singleton instance and creates all pools on Awake.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Create a pool for each configured entry in the ScriptablePool.
        foreach (var obj in scriptablePool.gameObjectsToPool)
            CreatePool(obj.gameObject, obj.quantity, obj.typePool);
    }

    /// <summary>
    /// Creates a new pool for the given prefab if one does not already exist.
    /// </summary>
    /// <param name="gameObject">Prefab to be pooled.</param>
    /// <param name="quantity">Initial quantity to instantiate.</param>
    /// <param name="poolType">Logical pool type.</param>
    private void CreatePool(GameObject gameObject, int quantity, TypePool poolType)
    {
        // Prevent duplicate pools for the same prefab.
        foreach (var pool in pools)
        {
            if (pool.prefabToSpawn == gameObject)
                return;
        }

        Pool newPool = new Pool(gameObject, quantity, poolType);
        pools.Add(newPool);
        InstantiatePool(newPool);
    }

    /// <summary>
    /// Instantiates all objects for the given pool and enqueues them as inactive.
    /// </summary>
    /// <param name="pool">The pool to populate.</param>
    private void InstantiatePool(Pool pool)
    {
        GameObject go;
        for (int i = 0; i < pool.quantity; i++)
        {
            go = Instantiate(pool.prefabToSpawn);
            pool.instantiatedGameObjects.Enqueue(go);
            go.SetActive(false);
        }
    }

    /// <summary>
    /// Retrieves an inactive object from the pool that matches the given prefab.
    /// </summary>
    /// <param name="gameObject">Prefab used as a key to find the pool.</param>
    /// <returns>
    /// A GameObject instance if available; otherwise null.
    /// Throws an exception if no pool exists for the prefab.
    /// </returns>
    public GameObject GetPoolObject(GameObject gameObject)
    {
        foreach (Pool pool in pools)
        {
            if (pool.prefabToSpawn == gameObject)
            {
                if (pool.instantiatedGameObjects.Count > 0)
                {
                    Debug.LogWarning("Object dequeued correctly.");
                    return pool.instantiatedGameObjects.Dequeue();
                }
                else
                {
                    Debug.LogWarning("Not enough GameObjects in the pool.");
                    return null;
                }
            }
        }

        // If we reach this point, no matching pool was found.
        throw new NotImplementedException("No pool has been created for the requested GameObject.");
    }

    /// <summary>
    /// Deactivates the given GameObject and returns it to the appropriate pool
    /// based on the provided TypePool value.
    /// </summary>
    /// <param name="gameObject">The instance to deactivate and enqueue.</param>
    /// <param name="typePool">Logical type used to find the correct pool.</param>
    public void DeactivatePoolObject(GameObject gameObject, TypePool typePool)
    {
        gameObject.SetActive(false);

        // Re-enqueue the object into the pool that matches the given type.
        foreach (Pool pool in pools)
        {
            if (pool.typePool == typePool)
            {
                pool.instantiatedGameObjects.Enqueue(gameObject);
                Debug.LogWarning("Object enqueued correctly.");
            }
        }
    }
}