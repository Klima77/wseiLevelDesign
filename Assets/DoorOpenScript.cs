using UnityEngine;

public class DoorOpenScript : MonoBehaviour
{
    #region FIELDS SERIALIZED

    [Header("Door Settings")]

    [Tooltip("Speed at which the door rotates when pushed (degrees per second).")]
    [SerializeField]
    private float rotationSpeed = 50.0f;

    [Tooltip("Whether the door opens outward (away from the player).")]
    [SerializeField]
    private bool openOutward = true;

    [Tooltip("Minimum angle the door can rotate to (degrees).")]
    [SerializeField]
    private float minAngle = -90.0f;

    [Tooltip("Maximum angle the door can rotate to (degrees).")]
    [SerializeField]
    private float maxAngle = 90.0f;

    [Tooltip("Speed at which the door's rotation smooths out (higher = faster smoothing).")]
    [SerializeField]
    private float smoothingSpeed = 5.0f;

    #endregion

    #region FIELDS

    private float initialYAngle;
    private float currentYAngle;
    private float targetRotationSpeed; // Target rotation speed (degrees per second) while pushing.
    private bool isBeingPushed;

    #endregion

    #region UNITY

    void Start()
    {
        // Store the initial Y rotation of the door.
        initialYAngle = transform.eulerAngles.y;
        currentYAngle = initialYAngle;
        targetRotationSpeed = 0f;
        isBeingPushed = false;
    }

    void Update()
    {
        if (isBeingPushed || Mathf.Abs(targetRotationSpeed) > 0.01f)
        {
            // Smoothly adjust the current Y angle based on the target rotation speed.
            currentYAngle += targetRotationSpeed * Time.deltaTime;

            // Clamp the angle within minAngle and maxAngle (relative to initial rotation).
            float clampedYAngle = Mathf.Clamp(currentYAngle, initialYAngle + minAngle, initialYAngle + maxAngle);
            currentYAngle = clampedYAngle;

            // Apply the smoothed rotation.
            Vector3 currentEuler = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentEuler.x, currentYAngle, currentEuler.z);

            // Gradually reduce the rotation speed if the door is not being pushed (for smooth stopping).
            if (!isBeingPushed)
            {
                targetRotationSpeed = Mathf.Lerp(targetRotationSpeed, 0f, Time.deltaTime * smoothingSpeed);
            }

            Debug.Log($"Rotating door: {targetRotationSpeed} degrees/sec (Current Y Angle: {currentYAngle})");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the player is pushing the door.
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = true;

            // Get the player's position and door's position.
            Vector3 playerPosition = collision.transform.position;
            Vector3 doorPosition = transform.position;

            // Calculate the vector from door to player in the XZ plane (ignore Y).
            Vector3 toPlayer = new Vector3(playerPosition.x - doorPosition.x, 0, playerPosition.z - doorPosition.z).normalized;

            // Get the door's local right axis (X-axis).
            Vector3 doorRight = transform.right;

            // Calculate which side the player is on.
            float dotProduct = Vector3.Dot(toPlayer, doorRight);
            Debug.Log($"Dot Product: {dotProduct} (Player is {(dotProduct > 0 ? "on right" : "on left")})");

            // Determine the rotation direction based on openOutward and side.
            float rotationSign;
            if (openOutward)
            {
                // Door opens away from the player.
                rotationSign = dotProduct > 0 ? -1f : 1f;
            }
            else
            {
                // Door opens toward the player.
                rotationSign = dotProduct > 0 ? 1f : -1f;
            }

            // Set the target rotation speed while the player is pushing.
            targetRotationSpeed = Mathf.Lerp(targetRotationSpeed, rotationSign * rotationSpeed, Time.deltaTime * smoothingSpeed);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Stop pushing when the player is no longer in contact.
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = false;
        }
    }

    // Debug visualization in the Editor.
    void OnDrawGizmos()
    {
        // Draw door axes for debugging.
        Vector3 doorPos = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(doorPos, doorPos + transform.right * 1f); // X-axis (right)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(doorPos, doorPos + transform.forward * 1f); // Z-axis (forward)
    }

    #endregion
}