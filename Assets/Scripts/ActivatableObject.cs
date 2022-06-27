using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour
{
    private bool m_spawned;

    // Empirical values found using printf :)
    private const float kSpawningZDist = 25.0f;
    private const float kDeSpawningZDist = -8.0f;

    private void OnEnable()
    {
        // hide the children
        foreach (Transform child in transform)
        {
            // Debug.Log("Deactivating: " + child.gameObject.name);
            child.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }

    void Update()
    {
        MouseController player = MouseController.gPlayer;

        if (!m_spawned)
        {
            // monitor the "spawning"
            float zDist = transform.position.z - player.mainCamera.transform.position.z;
            //Debug.Log("ZDist="+zDist);
            if ( zDist <= kSpawningZDist )
            {
                foreach (Transform child in transform)
                {
                    // Debug.Log("Activating: " + child.gameObject.name);
                    child.gameObject.SetActive(true);
                }
                m_spawned = true;
            }
        }
        else
        {
            float zDist = transform.position.z - player.mainCamera.transform.position.z;

            if (zDist <= kDeSpawningZDist)
            {
                // Debug.Log("Destroying: " + name);

                // object is out of screen now, delete the spawner (and all child objects)
                Destroy(gameObject);
            }
        }
    }
}
