using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple input-driven test script that requests objects from the ObjectPool
/// and demonstrates how to enable and position them at runtime.
/// </summary>
public class Picker : MonoBehaviour
{
    /// <summary>
    /// Prefabs used as keys for cube and sphere pools.
    /// </summary>
    [SerializeField] GameObject cube, sphere;

    // Update is called once per frame
    private void Update()
    {
        GameObject go = null;

        // Spawn a cube when pressing S.
        if (Input.GetKeyDown(KeyCode.S))
        {
            go = ObjectPool.Instance.GetPoolObject(cube);

            if (go != null)
            {
                // Randomize spawn position within a small range.
                go.transform.position = new Vector3(
                    Random.Range(-3f, 3f),
                    Random.Range(-3f, 3f),
                    Random.Range(-3f, 3f));

                // Let the object handle its own enable logic.
                go.GetComponent<IPoolable>().EnableObject();
            }
        }

        // Spawn a sphere when pressing D.
        if (Input.GetKeyDown(KeyCode.D))
        {
            go = ObjectPool.Instance.GetPoolObject(sphere);

            if (go != null)
            {
                // Let the object handle its own enable logic.
                go.GetComponent<IPoolable>().EnableObject();
            }
        }
    }
}