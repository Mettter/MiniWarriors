using UnityEngine;
using System.Collections.Generic;

public class ShamanWolf : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float triggerValue = 2f;
    public float yGizmoOffset = 0f;

    [Header("Effects")]
    public GameObject furyExplosion;
    public GameObject furyParticlesObject;

    [Header("Offsets")]
    public float yOffsetWave = 1f;
    public float yOffsetEffect = 0.5f;

    [Header("Cooldowns")]
    public float furyCooldown = 1f;  // Cooldown between giving Fury Points
    public float furyUnload = 0.5f;

    private Dictionary<GameObject, int> furyPoints = new Dictionary<GameObject, int>();

    [SerializeField] private string teamTag;
    [SerializeField] private string enemyTeamTag;

    private bool isBattleStarted = false;

    private Dictionary<GameObject, float> furyCooldownTimers = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> furyUnloadTimers = new Dictionary<GameObject, float>();

    private float furyUnloadTimer = 0f; 

    void Start()
    {  // Set opposite team
    }

    void Update()
    {
        if (transform.parent != null)
    {
        if (transform.parent.CompareTag("Team1"))
        {
            teamTag = "Team1";
            enemyTeamTag = "Team2";
        }
        else if (transform.parent.CompareTag("Team2"))
        {
            teamTag = "Team2";
            enemyTeamTag = "Team1";
        }
        else
        {
            Debug.LogWarning("ShamanWolf parent has an unexpected tag! Defaulting to Team1.");
            teamTag = "Team1";
            enemyTeamTag = "Team2";
        }
    }
    else
    {
        Debug.LogError("ShamanWolf script has no parent! Cannot determine team.");
    }

        if (Input.GetKeyDown(KeyCode.P))
    {
        isBattleStarted = true;
        Debug.Log("Battle Started!");
    }


        if (isBattleStarted)
        {
            DetectAllies();
            TryTriggerFury();
        }
    }

   private void DetectAllies()
{
    Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, detectionRange);

    foreach (Collider2D col in detectedObjects)
    {
        GameObject obj = col.gameObject;

        if (obj.CompareTag(teamTag) && obj.GetComponent<Projectile>() == null)
        {
            ApplyFuryPoint(obj); // Fury points are now applied with cooldown handling
        }
    }
}


    private void ApplyFuryPoint(GameObject target)
{
    if (!furyPoints.ContainsKey(target))
    {
        furyPoints[target] = 0;
    }

    if (!furyCooldownTimers.ContainsKey(target) || Time.time >= furyCooldownTimers[target] + furyCooldown)
    {
        furyPoints[target] += 1;
        furyCooldownTimers[target] = Time.time; // Reset the cooldown timer
        SpawnFuryParticles(target.transform.position);
    }
}

    private void TryTriggerFury()
{
    foreach (var kvp in new List<KeyValuePair<GameObject, int>>(furyPoints))
    {
        GameObject target = kvp.Key;
        int points = kvp.Value;

        if (target == null) continue;

        // Check if enough time has passed since the last fury unload attempt for this specific target
        if (Time.time < furyUnloadTimers.GetValueOrDefault(target, 0f) + furyUnload) continue;

        // Update the timer for the next fury unload attempt for this target
        furyUnloadTimers[target] = Time.time;

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance < triggerValue && HasEnemiesInRange())
        {
            furyPoints[target] -= 1;
            SpawnFuryExplosion(target.transform.position);

            if (furyPoints[target] <= 0)
            {
                furyPoints.Remove(target);
                furyUnloadTimers.Remove(target); // Clean up the timer for this target
            }
        }
    }
}


    private bool HasEnemiesInRange()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (Collider2D col in detectedObjects)
        {
            GameObject obj = col.gameObject;
            if (obj.CompareTag(enemyTeamTag) && obj.GetComponent<Projectile>() == null)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnFuryExplosion(Vector3 position)
{
    if (furyExplosion != null)
    {
        Vector3 spawnPos = new Vector3(position.x, position.y + yOffsetWave, position.z);
        GameObject spawnedExplosion = Instantiate(furyExplosion, spawnPos, Quaternion.identity);

        // Set the tag of the explosion to match the parent's tag
        spawnedExplosion.tag = transform.parent.tag;
    }
}


    private void SpawnFuryParticles(Vector3 position)
    {
        if (furyParticlesObject != null)
        {
            Vector3 spawnPos = new Vector3(position.x, position.y + yOffsetEffect, position.z);
            Instantiate(furyParticlesObject, spawnPos, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.3f);
        Vector3 gizmoPosition = new Vector3(transform.position.x, transform.position.y + yGizmoOffset, transform.position.z);
        Gizmos.DrawWireSphere(gizmoPosition, detectionRange);
    }
}
