using UnityEngine;
using System.Collections;

public class GraveyardSpawner : MonoBehaviour
{
    public GameObject spawnPrefab;  // The object to spawn
    public int lowestSpawnCount = 1;  // Minimum number of spawn iterations
    public int highestSpawnCount = 5;  // Maximum number of spawn iterations
    public float maxXOffset = 5f;  // Maximum offset along the X-axis from host position
    public float maxYOffset = 5f;  // Maximum offset along the Y-axis from host position
    public float spawnDelay = 1f;  // Delay between each spawn

    private string parentTag;  // Store the parent's tag

    private void Start()
    {
        // Get the tag from the parent object
        parentTag = gameObject.tag;

        // Start the spawning process
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        int spawnIterations = Random.Range(lowestSpawnCount, highestSpawnCount + 1);

        for (int i = 0; i < spawnIterations; i++)
        {
            float randomX = Random.Range(-maxXOffset, maxXOffset);
            float randomY = Random.Range(-maxYOffset, maxYOffset);
            Vector2 spawnPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);

            // Instantiate the spawnPrefab at the calculated position
            GameObject spawnedObject = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

            // Assign the same tag as the parent
            spawnedObject.tag = parentTag;

            // Wait before spawning the next object
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
