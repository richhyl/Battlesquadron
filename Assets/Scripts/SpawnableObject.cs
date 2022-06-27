using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour
{

    public GameObject m_objToSpawn;
    private GameObject m_spawnedObject;

    private bool m_spawned;

    // Empirical values found using printf :)
    private const float kSpawningZDist = 25.0f;
    private const float kDeSpawningZDist = -8.0f;


    // Start is called before the first frame update
    void Start()
    {
        if (m_objToSpawn == null )
        {
            // delete spawner if level designer forget to fill the spawn type :)
            Debug.Log("WARNING: SpawnableObject with empty m_objToSpawn field");
            Destroy(gameObject);
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

        if ( !m_spawned )
        {
            // monitor the "spawning"
            float zDist = transform.position.z - player.mainCamera.transform.position.z;
            //Debug.Log("ZDist="+zDist);
            if ( zDist <= kSpawningZDist )
            {
//                Debug.Log("Spawing enemy!");
                Vector3 pos = transform.position;
                Quaternion rot = Quaternion.Euler(0, 180, 0);
                pos.y = player.transform.position.y;        // force the same altitude than player
                m_spawnedObject = Instantiate(m_objToSpawn, pos, rot);
                m_spawned = true;
            }
        }
        else
        {
            if (m_spawnedObject == null)
            {
                // the spawned obj has been destroyed (maybe by hero weapon)
                // so we no longer need its spawner
                Destroy(gameObject);
            }
            else
            {
                // monitor "out of screen" dead of the spawned entity
                float zDist = m_spawnedObject.transform.position.z - player.mainCamera.transform.position.z;
                if ( zDist <= kDeSpawningZDist )
                {
//                    Debug.Log("Killing enemy!");
                    // object is out of screen now, delete both object & its spawner
                    Destroy(m_spawnedObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
