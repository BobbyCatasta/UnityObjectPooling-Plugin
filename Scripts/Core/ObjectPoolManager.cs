using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic pool manager that can handle any type T, including GameObjects,
/// using string keys for lookup and optional pool expansion.
/// </summary>
/// <typeparam name="T">The type to be pooled (e.g., GameObject).</typeparam>
public class ObjectPoolManager<T> : Singleton<ObjectPoolManager<T>>
{
    /// <summary>
    /// Initial list of poolable items, each configured with a key, object, quantity and growth flag.
    /// </summary>
    [SerializeField] private List<PoolableObject> poolableItems;

    /// <summary>
    /// Dictionary mapping keys to queues representing pools of objects.
    /// </summary>
    private Dictionary<string, Queue<T>> pools = new Dictionary<string, Queue<T>>();

    /// <summary>
    /// Initializes the pools when the component starts.
    /// </summary>
    private void Start()
    {
        InitPools();
    }

    /// <summary>
    /// Initializes all pools from the serialized list of PoolableObject items.
    /// </summary>
    protected void InitPools()
    {
        for (int i = 0; i < poolableItems.Count; i++)
            pools.Add(poolableItems[i].key, PopulatePool(poolableItems[i]));
    }

    /// <summary>
    /// Populates a single pool based on the given configuration item.
    /// </summary>
    /// <param name="item">The configuration for this pool.</param>
    /// <returns>A queue containing the pre-populated objects.</returns>
    private Queue<T> PopulatePool(PoolableObject item)
    {
        Queue<T> pool = new Queue<T>();

        for (int i = 0; i < item.quantity; i++)
        {
            // If T is a GameObject, instantiate and store it as inactive.
            if (item.pooledObject is GameObject)
            {
                GameObject go = Instantiate(item.pooledObject as GameObject, transform);
                go.SetActive(false);

                // Use dynamic to allow enqueuing as T without casting issues.
                dynamic obj = go;
                pool.Enqueue(obj);
            }
            else
            {
                // Non-GameObject types are simply enqueued as-is.
                pool.Enqueue(item.pooledObject);
            }
        }

        return pool;
    }

    /// <summary>
    /// Retrieves an instance associated with the given key.
    /// If no pool exists yet for that key, an attempt is made to create one from the configuration list.
    /// </summary>
    /// <param name="key">The textual key identifying the pool.</param>
    /// <returns>An instance of T, or default if none is available.</returns>
    public T GetInstanceOfObject(in string key)
    {
        if (pools.ContainsKey(key))
        {
            // If the pooled objects are GameObjects, differentiate active/inactive cases.
            if (pools[key].Peek() is GameObject)
            {
                GameObject go = pools[key].Peek() as GameObject;

                if (!go.activeInHierarchy)
                    return GiveInstance(key);
                else
                    return ExpandPool(key);
            }

            // Non-GameObject pool handling.
            if (pools[key].Count > 0)
                return pools[key].Dequeue();
            else
                ExpandPool(key);

            return default;
        }
        else
        {
            // Create a new pool from configuration if possible.
            PoolableObject item = GetItemFromListPoolableObjects(key);

            if (item != null && item.key == key)
            {
                pools.Add(key, PopulatePool(item));
                return GiveInstance(key);
            }

            Debug.Log("An item with the given key doesn't exist");
            return default;
        }
    }

    /// <summary>
    /// Tries to expand the pool associated with the given key, if allowed by its configuration.
    /// </summary>
    /// <param name="key">The textual key identifying the pool.</param>
    /// <returns>An instance of T, or default if the pool cannot be enlarged.</returns>
    private T ExpandPool(in string key)
    {
        PoolableObject item = GetItemFromListPoolableObjects(key);

        if (item.canGrow)
        {
            dynamic obj;

            // For GameObjects, instantiate a new one and enqueue it.
            if (item.pooledObject is GameObject)
            {
                GameObject go = Instantiate(item.pooledObject as GameObject, transform);
                go.SetActive(true);
                obj = go;
                pools[key].Enqueue(obj);
                return obj;
            }

            // For non-GameObject types, reuse an existing instance from the queue.
            obj = pools[key].Dequeue();
            return obj;
        }

        Debug.LogWarning("Pool can't be enlarged");
        return default;
    }

    /// <summary>
    /// Finds the configuration entry matching the given key (case-insensitive).
    /// </summary>
    /// <param name="key">The textual key identifying the configuration.</param>
    /// <returns>The matching PoolableObject configuration, or default if not found.</returns>
    private PoolableObject GetItemFromListPoolableObjects(in string key)
    {
        foreach (var item in poolableItems)
        {
            if (key.ToUpper() == item.key.ToUpper())
                return item;
        }

        Debug.LogWarning("No item has been found in the PoolableItems");
        return default;
    }

    /// <summary>
    /// Returns an instance from the pool for the given key and updates its state if needed.
    /// </summary>
    /// <param name="key">The textual key identifying the pool.</param>
    /// <returns>An instance of T.</returns>
    private T GiveInstance(in string key)
    {
        // For GameObjects, dequeue, activate, then re-enqueue.
        if (pools[key].Peek() is GameObject)
        {
            GameObject go = pools[key].Dequeue() as GameObject;
            go.SetActive(true);
            dynamic obj = go;
            pools[key].Enqueue(obj);
            return obj;
        }
        else
        {
            // For non-GameObject types, simply dequeue and return.
            return pools[key].Dequeue();
        }
    }

    /// <summary>
    /// Clears all pooled objects and resets the internal dictionary.
    /// </summary>
    public void ClearAllNullObjects()
    {
        pools.Clear();
    }

    /// <summary>
    /// Returns a list of all keys currently used by the internal pools dictionary.
    /// </summary>
    /// <returns>A list containing all pool keys.</returns>
    public List<string> GetListOfTags()
    {
        List<string> tags = new List<string>();

        foreach (var pool in pools)
            tags.Add(pool.Key);

        return tags;
    }

    /// <summary>
    /// Serializable configuration class used to define a single pool entry
    /// for the generic ObjectPoolManager.
    /// </summary>
    [Serializable]
    private class PoolableObject
    {
        /// <summary>
        /// Unique key used to identify this pool.
        /// </summary>
        public string key;

        /// <summary>
        /// The object to be pooled (could be GameObject or any type T).
        /// </summary>
        public T pooledObject;

        /// <summary>
        /// Initial number of instances to create.
        /// </summary>
        public int quantity;

        /// <summary>
        /// Whether this pool can grow when no instances are available.
        /// </summary>
        public bool canGrow;
    }
}