using UnityEngine;

public class EnemySpawner1 : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to spawn
    public float spawnDelay = 2f; // The time delay before spawning the object
    public int numberOfObjectsToSpawn = 1; // The number of objects to spawn
    public float timeBetweenSpawns = 0.5f; // The time delay between spawning each object

    private float timer = 0f; // A timer to track the elapsed time
    private int spawnedObjects = 0; // A counter to keep track of the number of spawned objects

    private void Update()
    {
        // Increment the timer by the time since the last frame
        timer += Time.deltaTime;

        // If the timer has exceeded the time between spawns and the maximum number of objects has not been reached, spawn the object and reset the timer
        if (timer >= timeBetweenSpawns && spawnedObjects < numberOfObjectsToSpawn)
        {
            SpawnObject();
            timer = 0f;
            spawnedObjects++;
        }
    }

    private void SpawnObject()
    {
        // Spawn the object at the spawner's position and rotation
        Instantiate(objectToSpawn, transform.position, transform.rotation);
    }
}
