using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject used to configure which GameObjects
/// should be pooled at startup, and in what quantity.
/// </summary>
[CreateAssetMenu(menuName = "ScriptablePool")]
public class ScriptablePool : ScriptableObject
{
    /// <summary>
    /// Array of objects to pool, each with prefab, quantity and pool type.
    /// </summary>
    public PoolableObject[] gameObjectsToPool;
}

/// <summary>
/// Represents a single prefab entry in a ScriptablePool configuration.
/// </summary>
[Serializable]
public struct PoolableObject
{
    [SerializeField] public GameObject gameObject;
    [SerializeField] public int quantity;
    [SerializeField] public TypePool typePool;
}