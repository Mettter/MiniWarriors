using System.Collections;
using UnityEngine;

public class SoulCollector : MonoBehaviour
{
    public GameObject soulCollectorClone; // Prefab to spawn at the position of the target
    public float overlapRadius = 99f; // The radius for the overlap circle
    public float delay = 1f; // Delay after the P key press

    private bool canDetect = false; // Flag to check if detection is allowed after P key press

    void Update()
    {
        // Check for P key press
        if (Input.GetKeyDown(KeyCode.P) && !canDetect)
        {
            // Start the coroutine for detecting and spawning soul after delay
            StartCoroutine(DetectAndSpawnSoul());
            canDetect = true;
        }
    }

    private IEnumerator DetectAndSpawnSoul()
    {
        // Wait for the specified delay after pressing P
        yield return new WaitForSeconds(delay);

        // Perform overlap circle detection in the specified range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapRadius);

        float smallestMaxHealth = Mathf.Infinity; // Initialize the smallest max health value
        GameObject targetWithSmallestHealth = null; // Store the target with the smallest max health

        // Loop through all colliders to find the correct target
        foreach (Collider2D col in colliders)
        {
            // Ignore projectiles
            if (col.GetComponent<Projectile>() != null)
                continue;

            // Check if the object belongs to the opposite team
            string targetTag = (gameObject.CompareTag("Team1")) ? "Team2" : "Team1";
            if (!col.CompareTag(targetTag))
                continue;

            // Try to get the HealthSystem component from the object
            HealthSystem healthSystem = col.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Compare the max health of the targets
                float maxHealth = healthSystem.maxHealth;
                if (maxHealth < smallestMaxHealth)
                {
                    smallestMaxHealth = maxHealth;
                    targetWithSmallestHealth = col.gameObject; // Store the target with the smallest max health
                }
            }
        }

        // If a valid target is found, spawn the SoulCollectorClone at its position with offset
        if (targetWithSmallestHealth != null)
        {
            Vector3 spawnPosition = targetWithSmallestHealth.transform.position;
            spawnPosition.y -= 0.25f; // Apply the Y offset

            // Instantiate the soulCollectorClone at the target's position
            GameObject spawnedObject = Instantiate(soulCollectorClone, spawnPosition, Quaternion.identity);

            // Assign the same tag as the parent object
            spawnedObject.tag = gameObject.tag;
        }

        // Reset the detection flag so it can be triggered again by pressing P
        canDetect = false;
    }
}
