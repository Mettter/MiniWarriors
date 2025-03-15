using System.Collections;
using UnityEngine;

public class StartSpawn : MonoBehaviour
{
    public GameObject spawnObject; // Object to spawn
    public float yOffset = 2f; // Y offset for the spawn position
    public float spawnDelay = 2f; // Delay before spawning the object

    private bool spawned = false; // To ensure it spawns only once when "P" is pressed

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !spawned)
        {
            // Start the spawn process with delay
            StartCoroutine(SpawnWithDelay());
        }
    }

    private IEnumerator SpawnWithDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(spawnDelay);

        // Calculate the spawn position with the yOffset
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        // Instantiate the spawnObject at the calculated position
        GameObject spawnedObject = Instantiate(spawnObject, spawnPosition, Quaternion.identity);

        // Set the tag of the spawned object to match the host object's tag
        spawnedObject.tag = gameObject.tag;

        // Set the spawned flag to true to prevent further spawns
        spawned = true;
    }
}
