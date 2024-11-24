using UnityEngine;

public class DamageBoosterWithTeleport : MonoBehaviour
{
    private Projectile projectile; // Reference to the Projectile component
    [SerializeField] private float boostedDamage = 9999f; // Damage value when boosted
    [SerializeField] private float teleportRange = 10f; // Number of tiles to teleport
    [SerializeField] private float teleportDuration = 0.5f; // Duration of teleportation effect

    private void Start()
    {
        // Get the Projectile component from the current GameObject
        projectile = GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError("No Projectile component found on this GameObject.");
        }
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
        if (projectile != null)
        {
            // Boost damage
            projectile.damage = boostedDamage;
            Debug.Log($"{projectile.name}'s damage set to {boostedDamage}");

            // Start teleportation
            StartCoroutine(TeleportRoutine());
        }
    }

    private System.Collections.IEnumerator TeleportRoutine()
    {
        // Teleport to a new position based on the teleport range
        Vector3 newPosition = transform.position + new Vector3(teleportRange, 0, 0); // Teleport only on the x-axis
        transform.position = newPosition;
        Debug.Log($"{gameObject.name} teleported by {teleportRange} tiles to position {transform.position}.");

        // Optionally, wait for the teleportation effect to finish before continuing
        yield return new WaitForSeconds(teleportDuration);
    }
}
