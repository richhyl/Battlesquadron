using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public float speed = 5f; // Player's movement speed

    void Update()
    {
        // Move the player forwards and backwards
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * speed * Time.deltaTime);

        // Move the player left and right
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
    }
}