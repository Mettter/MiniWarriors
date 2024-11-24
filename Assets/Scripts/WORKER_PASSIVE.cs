using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandpaBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float radius = 3f; // Radius of the detection zone
    [SerializeField] private float cooldownDuration = 5f; // Cooldown duration
    [SerializeField] private GameObject spawnPrefab; // Prefab to spawn

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.green; // Color of the detection circle

    private bool isBoostActivated = false; // Tracks if the P key has been pressed
    private float lastSpawnTime = -Mathf.Infinity; // Tracks the last spawn time

    private void Update()
    {
        // Check if the P key has been pressed to activate spawning
        if (Input.GetKeyDown(KeyCode.P))
        {
            isBoostActivated = true;
        }

        // Only proceed if boosting is activated and cooldown has passed
        if (!isBoostActivated || Time.time - lastSpawnTime < cooldownDuration) return;

        // Find the nearest valid target
        Collider2D nearestTarget = FindNearestTarget();

        if (nearestTarget != null)
        {
            SpawnPrefabAtTarget(nearestTarget);
            lastSpawnTime = Time.time; // Update cooldown timer
        }
    }

    private Collider2D FindNearestTarget()
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.y); // Center of the detection zone
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        Collider2D nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D obj in hitObjects)
        {
            // Skip objects with the same tag as the spawner
            if (obj.CompareTag(gameObject.tag)) continue;

            // Ensure the object belongs to Team1 or Team2
            if (obj.CompareTag("Team1") || obj.CompareTag("Team2"))
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);

                // Check if this object is the closest one found so far
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = obj;
                }
            }
        }

        return nearestTarget;
    }

    private void SpawnPrefabAtTarget(Collider2D target)
    {
        // Spawn the prefab at the target's position
        Vector3 spawnPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        GameObject spawnedObject = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

        // Set the same tag as the spawner object
        spawnedObject.tag = gameObject.tag;

        Debug.Log($"Spawned prefab at {target.name}'s position.");
    }

    private void OnDrawGizmos()
    {
        // Draw the detection radius in the editor for visualization
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
