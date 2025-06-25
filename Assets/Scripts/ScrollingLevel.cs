using UnityEngine;
using UnityEngine.Tilemaps; // Needed to access Tilemap component

public class ScrollingTilemap : MonoBehaviour
{
    [Tooltip("The speed at which the tilemap scrolls downwards.")]
    public float scrollSpeed = 2f; // Adjust in Inspector for desired speed

    [Tooltip("The camera that defines the viewable area.")]
    public Camera gameCamera; // Assign your Main Camera here

    [Tooltip("The Tilemap component to scroll.")]
    public Tilemap targetTilemap; // Assign the Tilemap child component here

    private float tilemapHeight; // The height of the visible portion of the tilemap
    private Vector3 startPosition; // The initial position of the tilemap

    void Start()
    {
        // Safety checks
        if (gameCamera == null)
        {
            gameCamera = Camera.main; // Try to find the main camera if not assigned
            if (gameCamera == null)
            {
                Debug.LogError("ScrollingTilemap: No Game Camera assigned and 'MainCamera' not found!", this);
                enabled = false; // Disable script if no camera
                return;
            }
        }

        if (targetTilemap == null)
        {
            targetTilemap = GetComponent<Tilemap>(); // Try to get Tilemap from this GameObject
            if (targetTilemap == null)
            {
                Debug.LogError("ScrollingTilemap: No Tilemap component assigned or found on this GameObject!", this);
                enabled = false; // Disable script if no tilemap
                return;
            }
        }

        startPosition = transform.position; // Store the initial position

        // Calculate the height of the tilemap in world units
        // This assumes an Orthographic camera and that your tiles are roughly 1x1 unit.
        // It gets the bounds of the painted tiles.
        tilemapHeight = targetTilemap.localBounds.size.y;

        // Alternative for tilemap height if you're sure of your grid/cell size:
        // tilemapHeight = targetTilemap.size.y * targetTilemap.cellSize.y;
    }

    void Update()
    {
        // Move the tilemap downwards
        // Time.deltaTime ensures consistent speed across different frame rates
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // --- Looping Logic ---
        // Calculate the camera's bottom edge in world coordinates
        // Assuming player's Z is at 0, and camera's Z is negative (e.g., -10)
        float cameraBottomWorldY = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, gameCamera.transform.position.z)).y;

        // If the bottom of the tilemap has scrolled past the bottom of the camera view,
        // reset it to the top to create a seamless loop.
        // We add an extra buffer (e.g., tilemapHeight / 2) to ensure it disappears fully before reappearing.
        // Or simply check if its top edge is off screen (below cameraBottomWorldY).

        // If the top edge of the tilemap (its current y + half its height) is below the camera's bottom edge,
        // meaning it's completely scrolled off-screen downwards.
        // Note: targetTilemap.transform.position is the center, so we add/subtract half height.
        if (targetTilemap.transform.position.y + (tilemapHeight / 2f) < cameraBottomWorldY)
        {
            // Calculate how far up it needs to go to be seamless.
            // Move it up by its own height. This assumes it's exactly one screen height worth of unique content.
            // For truly seamless looping, you'd usually have two identical tilemap sections.
            // A simpler approach for a single scrolling tilemap (assuming it's very long vertically):
            // Reset to a position relative to the startPosition + some multiple of its height.

            // A common way for single, repeating tilemaps:
            // Calculate how many "tilemap heights" have passed
            float distanceScrolled = startPosition.y - transform.position.y;
            if (distanceScrolled >= tilemapHeight)
            {
                transform.position = new Vector3(startPosition.x, startPosition.y + tilemapHeight, startPosition.z); // Or targetTilemap.transform.position.y + tilemapHeight * 2 for double looping
                startPosition = transform.position; // Update start position for next loop
            }

            // More robust looping (requires two tilemaps or a very long one):
            // You might have two identical tilemaps positioned one after another.
            // When the first one scrolls off, you move it to be after the second one.
            // This script assumes a single, potentially very long tilemap, and re-positions its top edge.
            // For practical looping, you'd move it by a multiple of its height that is off-screen.

            // A more direct simple loop for a single tilemap:
            // If the tilemap is essentially off-screen below, move it to be above the screen.
            // This is a quick fix assuming the tilemap itself is long enough.
            // For a smooth *repeat* loop (common in endless runners), you'd duplicate the tilemap
            // or use specific background techniques (Parallax scrolling usually involves 2+ backgrounds).
            float cameraTopWorldY = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, gameCamera.transform.position.z)).y;
            transform.position = new Vector3(transform.position.x, cameraTopWorldY + tilemapHeight / 2f, transform.position.z);
        }
    }
}