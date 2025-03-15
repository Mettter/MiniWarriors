using System.Collections;
using UnityEngine;

public class DashAnd : MonoBehaviour
{
    public float dashDistance = 5f;  // Distance of the dash
    public float dashSpeed = 20f;    // Speed of the dash
    public bool needSpawn = false;   // Determines whether to spawn objects
    public GameObject spawnObject;   // Prefab to spawn
    public float spawnDelay = 0.09f; // Time between each spawn (adjustable)
    public float startDelay = 2f;    // Delay before starting dash and spawn (adjustable)

    public GameObject spawnPosition1; // First spawn position (empty GameObject)
    public GameObject spawnPosition2; // Second spawn position (empty GameObject)

    private void Start()
    {
        // Start the entire dash and spawn system after a delay
        StartCoroutine(StartWithDelay());
    }

    private IEnumerator StartWithDelay()
    {
        // Wait for the specified start delay before starting the dash and spawn system
        yield return new WaitForSeconds(startDelay);

        // After the delay, start dash and spawn system simultaneously
        StartCoroutine(DashAndSpawn());
    }

    private IEnumerator DashAndSpawn()
    {
        // Start the dash and spawn system simultaneously
        Vector3 dashDirection = (gameObject.CompareTag("Team2")) ? -transform.right : transform.right;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        // Start the dash and spawn simultaneously
        StartCoroutine(ContinuousSpawn());
        yield return StartCoroutine(DashToPosition(dashTarget)); // Perform dash as coroutine
    }

    private IEnumerator DashToPosition(Vector3 targetPos)
    {
        // Perform dash movement while spawning continuously
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null; // Update every frame during dash
        }
    }

    private IEnumerator ContinuousSpawn()
    {
        // First spawn immediately at the start
        if (spawnPosition1 != null)
        {
            Instantiate(spawnObject, spawnPosition1.transform.position, Quaternion.identity).tag = gameObject.tag;
        }

        if (spawnPosition2 != null)
        {
            Instantiate(spawnObject, spawnPosition2.transform.position, Quaternion.identity).tag = gameObject.tag;
        }

        // Then continuously spawn with the defined delay
        while (true) // Keep spawning while the object is moving
        {
            // Spawn at spawnPosition1 if it's set
            if (spawnPosition1 != null)
            {
                Instantiate(spawnObject, spawnPosition1.transform.position, Quaternion.identity).tag = gameObject.tag;
            }

            // Spawn at spawnPosition2 if it's set
            if (spawnPosition2 != null)
            {
                Instantiate(spawnObject, spawnPosition2.transform.position, Quaternion.identity).tag = gameObject.tag;
            }

            // Wait for the spawn delay before spawning again
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
