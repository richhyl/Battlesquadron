using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public float speed = 5f; // Player's movement speed
    public float upSpeed = 2f; // Speed for moving the object up


    void Update()
    {
        // Move the player forwards and backwards
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * verticalInput * speed * Time.deltaTime);

        // Move the player left and right
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);

        // Move the object up
        transform.Translate(Vector3.up * upSpeed * Time.deltaTime);
    }
}
