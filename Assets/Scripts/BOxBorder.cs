using UnityEngine;

public class DamageBoosterWithTeleport : MonoBehaviour
{
    [SerializeField] private float boostedDamage = 9999f; // Damage value when boosted
    [SerializeField] private float teleportRange = 10f;   // Number of tiles to teleport
    [SerializeField] private float teleportDuration = 0.5f; // Duration of teleportation effect
    [SerializeField] private bool tpByY = false;          // Teleport by Y-coordinate if true, otherwise by X-coordinate

    private Vector3 originalPosition; // Store the original position

    private void Start()
    {
        // Store the original position at the start
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Check for P key press
        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyBoostAndTeleport();
        }
    }

    private void ApplyBoostAndTeleport()
    {
        // Start teleportation routine
        StartCoroutine(TeleportRoutine());
    }

    private System.Collections.IEnumerator TeleportRoutine()
    {
        // Determine the target position based on the teleport range and tpByY flag
        Vector3 targetPosition = tpByY 
            ? originalPosition + new Vector3(0, teleportRange, 0) 
            : originalPosition + new Vector3(teleportRange, 0, 0);

        // Teleport to the target position (first teleport)
        transform.position = targetPosition;
        Debug.Log($"{gameObject.name} teleported to {transform.position}.");

        // Wait for teleport duration
        yield return new WaitForSeconds(teleportDuration);

        // Teleport back to the original position
        transform.position = originalPosition;
        Debug.Log($"{gameObject.name} teleported back to original position {originalPosition}.");

        // Wait again for teleport duration
        yield return new WaitForSeconds(teleportDuration);

        // Now start a continuous loop to teleport back and forth
        while (true)
        {
            // Teleport to the target position
            transform.position = targetPosition;
            Debug.Log($"{gameObject.name} teleported to {transform.position}.");

            // Wait for teleport duration
            yield return new WaitForSeconds(teleportDuration);

            // Teleport back to the original position
            transform.position = originalPosition;
            Debug.Log($"{gameObject.name} teleported back to original position {originalPosition}.");

            // Wait again for teleport duration
            yield return new WaitForSeconds(teleportDuration);
        }
    }
}
