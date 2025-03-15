using UnityEngine;
using System.Collections;

public class Aging : MonoBehaviour
{
    public float maxAGE = 10f; // Maximum lifetime before aging (in seconds)
    public GameObject olderVersion; // The object to spawn after aging
    public GameObject effectObject; // The effect to spawn when P is pressed
    public float yOffset = 1f; // Y offset for the spawned older version

    private bool pKeyPressed = false; // Track if P has been pressed
    private bool agingStarted = false; // Ensure aging starts only once

    private void Update()
    {
        // Check if P is pressed and aging hasn't started yet
        if (Input.GetKeyDown(KeyCode.P) && !pKeyPressed)
        {
            pKeyPressed = true;
            SpawnEffect();
            StartCoroutine(AgingProcess());
        }
    }

    private IEnumerator AgingProcess()
    {
        if (agingStarted) yield break; // Prevent multiple starts
        agingStarted = true;

        yield return new WaitForSeconds(maxAGE);

        if (this != null) // If the object still exists
        {
            AgeObject();
        }
    }

    private void SpawnEffect()
    {
        if (effectObject != null)
        {
            Instantiate(effectObject, transform.position, Quaternion.identity);
        }
    }

    private void AgeObject()
    {
        if (olderVersion != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            GameObject spawnedObject = Instantiate(olderVersion, spawnPosition, Quaternion.identity);
            
            // Set the same tag as the current object
            spawnedObject.tag = gameObject.tag;
        }

        Destroy(gameObject); // Destroy current object
    }
}
