using UnityEngine;

public class SpawnOnCollision : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab of the object to spawn

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}
