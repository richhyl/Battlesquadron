using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target and Offset")]
    [Tooltip("The GameObject the camera should follow.")]
    public Transform target; // The object the camera will follow

    [Tooltip("The offset distance from the target (X and Y control position, Z controls camera depth).")]
    // Adjusted default offset for a common 2D setup
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Default for 2D: X/Y can be 0, Z is negative for depth

    [Header("Smoothing Properties")]
    [Tooltip("How smoothly the camera follows the target. Higher values mean faster follow, less delay.")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f; // Controls the "delay" - smaller value = more delay

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target is not assigned. Please assign a target GameObject in the Inspector.", this);
            return;
        }

        // Calculate the desired position based on the target's position and the offset
        // Ensure the Z position of the desiredPosition is maintained by the offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;

        // For 2D, you usually do NOT want the camera to look at the target.
        // It keeps its default forward orientation (looking along its own Z-axis).
        // If uncommented, it might cause the camera to rotate if your 2D player rotates.
        // transform.LookAt(target);
    }
}