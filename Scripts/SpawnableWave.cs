using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableWave : MonoBehaviour
{

    public GameObject m_objToSpawn;
    public int m_waveCount;
    public float m_waveRadius;
    public int m_waveSpaceDegree;
    public float m_angleSpeed;



    private GameObject[] m_spawnedObjects;

    private bool m_spawned;
    private float m_angle;

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
        else
        {
            m_spawnedObjects = new GameObject[m_waveCount];
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(2, 2, 2));
    }

    private int    SetWavePos(float y)
    {
        Quaternion rot = Quaternion.Euler(0, 180, 0);
        float angle = m_angle;
        int count = 0;
        for (int i = 0; i < m_waveCount; i++)
        {
            if ( m_spawnedObjects[i] != null )
            {
                Vector3 pos = transform.position;
                pos.x = transform.position.x + m_waveRadius * Mathf.Cos(angle);
                pos.y = y;
                pos.z = transform.position.z + m_waveRadius * Mathf.Sin(angle);
                m_spawnedObjects[i].transform.position = pos;
                m_spawnedObjects[i].transform.rotation = rot;
                count++;
            }
            angle += (float)m_waveSpaceDegree * 3.1415926f / 180.0f;
        }
        return count;
    }


    void Update()
    {

        m_angle += m_angleSpeed * 3.1415926f / 180.0f * Time.deltaTime;

        MouseController player = MouseController.gPlayer;
        float zDist = transform.position.z - player.mainCamera.transform.position.z;

        if ( !m_spawned )
        {
            // monitor the "spawning"
            //Debug.Log("ZDist="+zDist);
            if ( zDist <= kSpawningZDist + m_waveRadius )
            {
                for (int i=0;i<m_waveCount;i++)
                    m_spawnedObjects[i] = Instantiate(m_objToSpawn);

                SetWavePos(player.transform.position.y);

                m_spawned = true;
            }
        }
        else
        {
            int count = SetWavePos(player.transform.position.y);
            if (count == 0)
            {
                // All the spawned obj has been destroyed (maybe by hero weapon)
                // so we no longer need its spawner
                Destroy(gameObject);
            }
            else
            {
                // monitor "out of screen" dead of the spawned entity
                if ( zDist <= kDeSpawningZDist - m_waveRadius)
                {
                    // object is out of screen now, delete both object & its spawner
                    for (int i=0;i<m_waveCount;i++)
                    {
                        if ( m_spawnedObjects[i] != null )
                            Destroy(m_spawnedObjects[i]);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
