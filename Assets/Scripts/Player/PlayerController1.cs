using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // Player's horizontal/vertical movement speed
    public float upSpeed = 2f; // Speed for moving the object up (if applicable, consider if this should also be clamped)

    [Header("Boundary Settings")]
    [Tooltip("Adjust these values to create padding from the screen edges.")]
    public float paddingX = 0.5f; // How far from the left/right screen edge the player can go (half width of player)
    public float paddingY = 0.5f; // How far from the top/bottom screen edge the player can go (half height of player)

    private Camera mainCamera; // Reference to the main camera
    private Vector2 screenMinWorld; // Bottom-left screen boundary in world coordinates
    private Vector2 screenMaxWorld; // Top-right screen boundary in world coordinates

    void Start()
    {
        // Get a reference to the main camera.
        // It's good practice to ensure your camera is tagged "MainCamera" in Unity.
        mainCamera = Camera.main;

        // --- Calculate Screen Boundaries in World Coordinates ---
        // These calculations should ideally happen once, or when the screen resolution changes.
        // We use the player's Z position for the conversion to get accurate world points at the player's depth.
        float playerZ = transform.position.z;

        // Bottom-left corner of the screen (0,0 viewport) converted to world coordinates
        screenMinWorld = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, playerZ - mainCamera.transform.position.z));
        // Top-right corner of the screen (1,1 viewport) converted to world coordinates
        screenMaxWorld = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, playerZ - mainCamera.transform.position.z));

        // You could also directly use orthographicSize if your player's Z is 0 and camera's Z is fixed.
        // For a true 2D orthographic camera:
        // float camHalfHeight = mainCamera.orthographicSize;
        // float camHalfWidth = mainCamera.aspect * camHalfHeight;
        // screenMinWorld.x = mainCamera.transform.position.x - camHalfWidth;
        // screenMaxWorld.x = mainCamera.transform.position.x + camHalfWidth;
        // screenMinWorld.y = mainCamera.transform.position.y - camHalfHeight;
        // screenMaxWorld.y = mainCamera.transform.position.y + camHalfHeight;

        // Optional: Dynamically calculate padding based on player's sprite bounds
        // This makes it so the *edge* of the sprite hits the screen border, not its center.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            paddingX = sr.bounds.extents.x;  // Half width of the player's sprite
            paddingY = sr.bounds.extents.y;  // Half height of the player's sprite
        }
        else
        {
            Debug.LogWarning("PlayerController1: No SpriteRenderer found. Default paddingX/Y used. Player might go slightly off screen.", this);
            // If no SpriteRenderer, the default paddingX/Y values you set will be used.
            // You might need to manually adjust paddingX/Y in the Inspector.
        }
    }

    void Update()
    {
        // --- Player Movement ---
        // Move the player forwards and backwards (using 'up' for vertical movement)
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * verticalInput * speed * Time.deltaTime);

        // Move the player left and right (using 'right' for horizontal movement)
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);

        // Move the object up (your existing continuous upward movement,
        // if this is intended to be separate from input-based vertical)
        transform.Translate(Vector3.up * upSpeed * Time.deltaTime);

        // --- Clamping Logic ---
        Vector3 playerPos = transform.position;

        // Clamp X position: Restrict player's X between the left screen edge plus padding
        // and the right screen edge minus padding.
        playerPos.x = Mathf.Clamp(playerPos.x, screenMinWorld.x + paddingX, screenMaxWorld.x - paddingX);

        // Clamp Y position: Restrict player's Y between the bottom screen edge plus padding
        // and the top screen edge minus padding.
        playerPos.y = Mathf.Clamp(playerPos.y, screenMinWorld.y + paddingY, screenMaxWorld.y - paddingY);

        // Apply the clamped position back to the player's transform
        transform.position = playerPos;
    }
}