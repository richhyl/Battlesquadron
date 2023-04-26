using UnityEngine;

public class testmovement : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Adjust this value to control the speed of movement
    private float timeElapsed = 0.0f; // Tracks how much time has passed

    void Update()
    {
        if (timeElapsed >= 1.0f)
        {
            // Move the game object on a 45-degree angle
            transform.position += new Vector3(-moveSpeed * Mathf.Cos(Mathf.PI / 4.0f) * Time.deltaTime, -moveSpeed * Mathf.Sin(Mathf.PI / 4.0f) * Time.deltaTime, 0);
        }
        else
        {
            // Move the game object on the -y axis
            transform.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
            timeElapsed += Time.deltaTime;
        }
    }
}
