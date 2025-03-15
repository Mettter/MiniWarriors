using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DashAndChas : MonoBehaviour
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

    // Energy system
    public float energyUsagePerDash = 10f; // Energy cost per dash // Rate of energy recovery per second

    // Mana System reference // Reference to ManaSystem script
    private bool hasPressedP = false; // Flag to check if P was pressed
    private bool isDashing = false; 
    public bool PROJECTILE = false;
    
    public ManaSystem manaSystem;  // Flag to track dash status

    private void Start()
    {
        if(PROJECTILE)
        {
            hasPressedP = true;
        }
    }

    private void Update()
    {

        // Check for 'P' key press to enable dashing
        if (Input.GetKeyDown(KeyCode.P) && !hasPressedP)
        {
            hasPressedP = true; // Set the flag when P is pressed
        }

        // Check if player has enough mana to dash
        if (manaSystem != null && manaSystem.currentMana >= energyUsagePerDash && hasPressedP)
        {
            StartCoroutine(DashAndMoveToEnemies());
            manaSystem.currentMana -= energyUsagePerDash;
        }
    }

    private IEnumerator DashAndMoveToEnemies()
    {
        if (!hasPressedP || manaSystem == null || manaSystem.currentMana < energyUsagePerDash)
        {
            yield break; // Do not proceed with dashing if P hasn't been pressed or not enough mana
        }

        // Deduct mana for dash
        manaSystem.currentMana -= energyUsagePerDash;

        isDashing = true; // Set dashing flag to true

        // Determine dash direction
        Vector3 dashDirection = (gameObject.CompareTag("Team2")) ? -transform.right : transform.right;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        // Dash in the determined direction if enough energy
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
    // Move to target position first
    yield return StartCoroutine(MoveToPosition(target.position));
    enemyList.Remove(target); // Remove target to avoid selecting it again
    bounceCount++;

    // Spawn the object with the -0.25 Y offset and same tag as the runner
    Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
    GameObject spawnedObject = Instantiate(spawnObject, spawnPosition, Quaternion.identity);

    // Set the tag of the spawned object to match the runner's tag
    spawnedObject.tag = gameObject.tag;

    Debug.Log("Spawned object with offset and tag.");
}

            yield return new WaitForSeconds(waitTime);
        }

        isDashing = false; // Reset dashing flag after completing the dashes and movements
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

        return closestObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with an enemy and spawn object
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (spawnObject != null && needSpawn)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
                Debug.Log("Spawned object upon collision.");
            }
        }
    }
}
