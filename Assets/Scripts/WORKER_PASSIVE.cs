using System.Collections;
using UnityEngine;

public class GrandpaBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private GameObject spawnPrefab; // Prefab to spawn
    [SerializeField] private Vector2[] spawnCoordinates; // Array of spawn coordinates (x, y)
    [SerializeField] private float yOffset = -0.25f; // Y offset for the spawn position, adjustable in Inspector

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.green; // Color of the detection circle

    private bool hasSpawned = false; // Tracks if the prefab has already been spawned

    private void Update()
    {
        // Check if the P key has been pressed and if the prefab hasn't been spawned yet
        if (Input.GetKeyDown(KeyCode.P) && !hasSpawned)
        {
            StartCoroutine(SpawnPrefabsWithDelay());
        }
    }

    private IEnumerator SpawnPrefabsWithDelay()
    {
        // Wait for 0.1 seconds before spawning
        yield return new WaitForSeconds(0.1f);

        // Check if this object has been destroyed during the delay
        if (this == null) yield break;

        if (spawnCoordinates.Length == 0)
        {
            Debug.LogError("No spawn coordinates assigned. Please assign spawn coordinates in the Inspector.");
            yield break;
        }

        foreach (Vector2 coordinate in spawnCoordinates)
        {
            // Adjust the spawn position with the Y offset
            Vector2 adjustedSpawnPosition = coordinate;
            adjustedSpawnPosition.y += yOffset;

            // Spawn the prefab at the adjusted position (convert Vector2 to Vector3)
            Vector3 spawnPosition3D = new Vector3(adjustedSpawnPosition.x, adjustedSpawnPosition.y, 0);
            GameObject spawnedObject = Instantiate(spawnPrefab, spawnPosition3D, Quaternion.identity);

            // Set the same tag as the spawner object
            spawnedObject.tag = gameObject.tag;

            // Log spawn information
            Debug.Log($"Spawned prefab at position: {spawnPosition3D}");
        }

        // Mark that the prefabs have been spawned
        hasSpawned = true;
    }

    private void OnDrawGizmos()
    {
        // Draw gizmos for all spawn coordinates
        if (spawnCoordinates != null)
        {
            foreach (Vector2 coordinate in spawnCoordinates)
            {
                Gizmos.color = Color.red;
                Vector3 gizmoPosition = new Vector3(coordinate.x, coordinate.y + yOffset, 0);
                Gizmos.DrawWireSphere(gizmoPosition, 0.1f);
            }
        }
    }
}
