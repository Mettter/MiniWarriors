using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DashAndChase : MonoBehaviour
{
    public float dashDistance = 5f;  // Distance of the dash
    public float dashSpeed = 20f;    // Speed of the dash
    public float moveSpeed = 5f;     // Speed while chasing enemies
    public float waitTime = 0.5f;    // Time between each move
    public bool isSupport = false;   // Check if the unit is support
    private List<Transform> enemyList = new List<Transform>(); // Stores multiple enemies
    private int bounceCount = 0;     // Tracks number of dashes

    public int seekCount = 3;

    // Spawning system
    public bool needSpawn = false;   // Determines whether to spawn objects
    public GameObject spawnObject;   // Prefab to spawn
    public float spawnDelay = 1f;    // Time between each spawn

    private void Start()
    {
        StartCoroutine(DashAndMoveToEnemies());
        if (needSpawn)
        {
            StartCoroutine(SpawnObjects());
        }
    }

    private IEnumerator DashAndMoveToEnemies()
    {
        // Determine dash direction
        Vector3 dashDirection = (gameObject.CompareTag("Team2")) ? -transform.right : transform.right;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        // Dash in the determined direction
        yield return StartCoroutine(DashToPosition(dashTarget));

        // Find all enemies before moving
        FindAllEnemies();

        // Move to different enemies (max 3 times)
        for (int i = 0; i < seekCount; i++)
        {
            if (enemyList.Count == 0) break; // Stop if no enemies left

            Transform target = FindNearestFromList();

            if (target != null)
            {
                yield return StartCoroutine(MoveToPosition(target.position));
                enemyList.Remove(target); // Remove target to avoid selecting it again
                bounceCount++;
            }

            yield return new WaitForSeconds(waitTime);
        }

        // Destroy object if it bounced 3 times or there are no more enemies
        Destroy(gameObject);
    }

    private IEnumerator DashToPosition(Vector3 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
{
    // Apply a 0.25 offset to the Y position *here*, instead of modifying the target's position
    Vector3 adjustedTargetPos = new Vector3(targetPos.x, targetPos.y - 0.25f, targetPos.z);

    while (Vector2.Distance(transform.position, adjustedTargetPos) > 0.1f)
    {
        transform.position = Vector2.MoveTowards(transform.position, adjustedTargetPos, moveSpeed * Time.deltaTime);
        yield return null;
    }
}


    private void FindAllEnemies()
    {
        enemyList.Clear();
        string targetTag = isSupport ? gameObject.tag : (gameObject.CompareTag("Team1") ? "Team2" : "Team1");
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<Projectile>() != null) continue;
            if (obj.GetComponent<Invisible>() != null) continue;
            if (obj.CompareTag("Untarget")) continue;

            NearestEnemy unitMovement = obj.GetComponent<NearestEnemy>();
            if (unitMovement != null && unitMovement.isInvisible) continue;

            enemyList.Add(obj.transform);
        }
    }

    private Transform FindNearestFromList()
{
    float shortestDistance = Mathf.Infinity;
    Transform closestObject = null;

    foreach (Transform enemy in enemyList)
    {
        float distance = Vector2.Distance(transform.position, enemy.position);
        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            closestObject = enemy;
        }
    }

    if (closestObject != null)
    {
        // Create a new position with the target's Y reduced by 0.25
        Vector3 adjustedTargetPos = new Vector3(closestObject.position.x, closestObject.position.y, closestObject.position.z);
        
        // Now you can return the adjusted position for the movement logic
        closestObject.position = adjustedTargetPos;
    }

    return closestObject;
}

    private IEnumerator SpawnObjects()
    {
        while (true) // Loop until the object is destroyed
        {
            yield return new WaitForSeconds(spawnDelay);

            if (spawnObject != null)
            {
                GameObject spawned = Instantiate(spawnObject, transform.position, Quaternion.identity);
                spawned.tag = gameObject.tag; // Inherit the tag from the original object
            }
        }
    }
}
