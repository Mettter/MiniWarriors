using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RATKING : MonoBehaviour
{
    public GameObject soulCollectorClone; // Prefab to spawn at the position of the target
    public float overlapRadius = 99f; // The radius for the overlap circle
    public float delay = 1f; // Delay after the P key press
    public float additionalSpawnDelay = 1f; // Additional delay before spawning at previous position
    public int spawnCount = 3; // Number of times to spawn
    public float spawnCooldown = 2f; // Delay between spawns
    public float yOffset = -0.25f; // Y-axis offset for spawn position

    private bool canDetect = false; // Flag to check if detection is allowed after P key press
    private Dictionary<GameObject, Vector3> previousPositions = new Dictionary<GameObject, Vector3>();

    void Update()
    {
        // Check for P key press
        if (Input.GetKeyDown(KeyCode.P) && !canDetect)
        {
            StartCoroutine(DetectAndSpawnSoul());
            canDetect = true;
        }
    }

    private IEnumerator DetectAndSpawnSoul()
    {
        yield return new WaitForSeconds(delay);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapRadius);

        float highestMaxHealth = 0f; // Track the highest max health
        GameObject targetWithHighestHealth = null;

        foreach (Collider2D col in colliders)
        {
            if (col.GetComponent<Projectile>() != null)
                continue;

            string targetTag = (gameObject.CompareTag("Team1")) ? "Team2" : "Team1";
            if (!col.CompareTag(targetTag))
                continue;

            HealthSystem healthSystem = col.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                float maxHealth = healthSystem.maxHealth;
                if (maxHealth > highestMaxHealth)
                {
                    highestMaxHealth = maxHealth;
                    targetWithHighestHealth = col.gameObject;
                }
            }
        }

        if (targetWithHighestHealth != null)
        {
            GameObject target = targetWithHighestHealth;
            StartCoroutine(SpawnMultipleTimes(target));
        }

        canDetect = false;
    }

    private IEnumerator SpawnMultipleTimes(GameObject target)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Check if the current target is still valid (not destroyed)
            if (target == null)
            {
                // Use your existing method to find a new target
                target = FindNewTarget();

                // If no new target is found, break out of the loop
                if (target == null)
                {
                    break; // Exit if no valid target is found
                }
            }

            if (target != null)
            {
                Vector3 pastPosition = target.transform.position;
                yield return new WaitForSeconds(additionalSpawnDelay);

                pastPosition.y += yOffset; // Apply Y offset
                GameObject spawnedObject = Instantiate(soulCollectorClone, pastPosition, Quaternion.identity);
                spawnedObject.tag = gameObject.tag;

                yield return new WaitForSeconds(spawnCooldown);
            }
        }
    }

    // Your method to find a new target
    private GameObject FindNewTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapRadius);
        float highestMaxHealth = 0f;
        GameObject newTarget = null;

        foreach (Collider2D col in colliders)
        {
            if (col.GetComponent<Projectile>() != null)
                continue;

            string targetTag = (gameObject.CompareTag("Team1")) ? "Team2" : "Team1";
            if (!col.CompareTag(targetTag))
                continue;

            HealthSystem healthSystem = col.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                float maxHealth = healthSystem.maxHealth;
                if (maxHealth > highestMaxHealth)
                {
                    highestMaxHealth = maxHealth;
                    newTarget = col.gameObject;
                }
            }
        }

        return newTarget; // Return the new target, or null if none found
    }
}
