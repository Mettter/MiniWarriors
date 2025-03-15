using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject spawnObjectPrefab; // Object to spawn
    [SerializeField] private Transform spawnPoint1; // Spawn location 1
    [SerializeField] private Transform spawnPoint2; // Spawn location 2
    [SerializeField] private Transform spawnPoint3; // Spawn location 3
    [SerializeField] private Transform spawnPoint4; // Spawn location 4
    [SerializeField] private float yOffset = -0.25f; // Y offset for spawning
    [SerializeField] private float firstSpawnDelay = 0f; // Delay before first spawn (default 0)
    [SerializeField] private float spawnCooldown = 2f; // Cooldown between each spawn
    [SerializeField] private bool multipleSpawns = false;
    [SerializeField] private bool TwoSpawnPoints = false; // Use only 2 spawn points
    [SerializeField] private int spawnLimit = -1; // Maximum number of spawns (-1 = infinite)

    private bool hasSpawned = false; // Prevents multiple spawns if `multipleSpawns` is false
    private int spawnCount = 0; // Tracks how many times objects have been spawned

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!multipleSpawns && !hasSpawned)
            {
                StartCoroutine(SpawnWithDelay(firstSpawnDelay));
                hasSpawned = true; // Prevent further spawning
            }
            else if (multipleSpawns)
            {
                StartCoroutine(SpawnObjectsContinuously());
            }
        }
    }

    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        if (spawnObjectPrefab == null || spawnPoint1 == null || spawnPoint2 == null)
        {
            Debug.LogWarning("Spawn object or required spawn points (1,2) are not assigned!");
            return;
        }

        if (spawnLimit != -1 && spawnCount >= spawnLimit) return; // Stop if limit is reached

        // Spawn at only 2 locations
        SpawnAtLocation(spawnPoint1);
        SpawnAtLocation(spawnPoint2);

        if (!TwoSpawnPoints)
        {
            if (spawnPoint3 == null || spawnPoint4 == null)
            {
                Debug.LogWarning("Spawn points 3 and 4 are not assigned but required!");
                return;
            }
            SpawnAtLocation(spawnPoint3);
            SpawnAtLocation(spawnPoint4);
        }

        spawnCount++; // Increment spawn count
    }

    private void SpawnAtLocation(Transform spawnPoint)
    {
        Vector3 spawnPosition = spawnPoint.position + new Vector3(0f, yOffset, 0f);
        GameObject spawnedObject = Instantiate(spawnObjectPrefab, spawnPosition, Quaternion.identity);

        // Set tag of spawned object to match the host's tag
        spawnedObject.tag = gameObject.tag;
    }

    private IEnumerator SpawnObjectsContinuously()
    {
        yield return new WaitForSeconds(firstSpawnDelay); // Wait before first spawn

        while (multipleSpawns && (spawnLimit == -1 || spawnCount < spawnLimit))
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnCooldown); // Wait before next spawn
        }
    }
}
