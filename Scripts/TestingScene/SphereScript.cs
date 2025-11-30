using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example implementation of IPoolable for a sphere.
/// Automatically disables itself after a random delay and returns
/// to the corresponding pool.
/// </summary>
public class SphereScript : MonoBehaviour, IPoolable
{
    /// <summary>
    /// Pool type used to route this instance back to the correct pool.
    /// </summary>
    [SerializeField] private TypePool typePool;

    /// <summary>
    /// Starts a coroutine each time the object is enabled, which will
    /// automatically disable and return it to the pool after a delay.
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(DisableCoroutine());
    }

    /// <summary>
    /// Returns this GameObject to the ObjectPool and deactivates it.
    /// </summary>
    public void DisableObject()
    {
        ObjectPool.Instance.DeactivatePoolObject(gameObject, typePool);
    }

    /// <summary>
    /// Called when the object is taken from the pool and should be active.
    /// </summary>
    public void EnableObject()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Waits for a random amount of time, then returns the object to the pool.
    /// </summary>
    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0, 2));
        DisableObject();
    }
}
