using System.Collections;
using UnityEngine;

public class GrandpaBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float radius = 3f; // Radius of the detection zone (currently unused)
    [SerializeField] private GameObject spawnPrefab; // Prefab to spawn
    [SerializeField] private Transform spawnPoint; // The transform of the spawn point
    [SerializeField] private float yOffset = -0.25f; // Y offset for the spawn position, adjustable in Inspector

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.green; // Color of the detection circle

    private bool hasSpawned = false; // Tracks if the prefab has already been spawned

    private void Update()
    {
        // Check if the P key has been pressed and if the prefab hasn't been spawned yet
        if (Input.GetKeyDown(KeyCode.P) && !hasSpawned)
        {
            StartCoroutine(SpawnPrefabWithDelay());
        }
    }

    private IEnumerator SpawnPrefabWithDelay()
    {
        // Wait for 0.1 seconds before spawning
        yield return new WaitForSeconds(0.1f);

        // Check if this object has been destroyed during the delay
        if (this == null) yield break;

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is not assigned. Please assign a spawn point in the Inspector.");
            yield break;
        }

        // Adjust the spawn position with the Y offset
        Vector3 adjustedSpawnPosition = spawnPoint.position;
        adjustedSpawnPosition.y += yOffset;

        // Spawn the prefab at the adjusted position
        GameObject spawnedObject = Instantiate(spawnPrefab, adjustedSpawnPosition, spawnPoint.rotation);

        // Set the same tag as the spawner object
        spawnedObject.tag = gameObject.tag;

        // Log spawn information
        Debug.Log($"Spawned prefab at spawn point with Y offset: {spawnPoint.name}, Position: {adjustedSpawnPosition}");

        // Mark that the prefab has been spawned
        hasSpawned = true;
    }

    private void OnDrawGizmos()
    {
        // Draw the detection radius in the editor for visualization
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
