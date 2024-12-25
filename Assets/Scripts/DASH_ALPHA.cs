using System.Collections;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f; // Distance to dash
    [SerializeField] private float dashDuration = 0.2f; // Duration of the dash

    private bool isDashing = false; // Whether the object is currently dashing
    private Vector3 originalPosition; // To store the starting position during the dash

    private void Update()
    {
        // Check for key press and ensure the object is not already dashing
        if (Input.GetKeyDown(KeyCode.K) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true; // Set dashing state
        originalPosition = transform.position; // Record the starting position

        float elapsed = 0f; // Timer for the dash
        Vector3 targetPosition = originalPosition + new Vector3(dashDistance, 0, 0); // Target position

        while (elapsed < dashDuration)
        {
            // Smoothly interpolate the object's position to the target position over the dash duration
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / dashDuration);
            elapsed += Time.deltaTime; // Increment the timer
            yield return null;
        }

        // Ensure the object ends exactly at the target position
        transform.position = targetPosition;

        isDashing = false; // Reset dashing state
    }
}