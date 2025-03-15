using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;  // Damage amount
    public string teamTag; // Tag for the projectile's team
    public GameObject projectileParticles;  // The particle effect prefab to spawn
    private TrailRenderer trailRenderer; // Reference to the TrailRenderer
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    [SerializeField] public bool isPiersesThoughTarget = false;
    [SerializeField] public bool isIgnoresArmorArrow = false;
    [SerializeField] public bool cursesEnemies = false;
    [SerializeField] public int curseAmountP = 0;
    [SerializeField] public bool isProjectileStun = false; // Determines if this projectile stuns
    [SerializeField] public float stunAmount = 2f; // Stun duration in seconds
    [SerializeField] public bool isDamagesMult = false; // New boolean for multiple hits
    private Dictionary<Collider2D, float> damageTimers = new Dictionary<Collider2D, float>(); // Tracks last hit time per enemy

    private void Start()
    {
        // Ensure the collider is set to trigger if we are using OnTriggerEnter2D
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogError("No Collider2D found on the projectile.");
        }

        // Optionally set the teamTag based on the tag of the projectile
        teamTag = gameObject.tag;

        // Get the TrailRenderer component
        trailRenderer = GetComponent<TrailRenderer>();

        // Get the SpriteRenderer component to flip the sprite
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Flip the sprite if the teamTag is "Team2"
        if (teamTag == "Team2" && spriteRenderer != null)
        {
            spriteRenderer.flipX = true; // Flip sprite horizontally
        }

        // Ignore collision with other projectiles of the same tag
        IgnoreProjectileCollision();
    }

    private void IgnoreProjectileCollision()
    {
        // Find all colliders in the scene with the same tag as this projectile
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag(teamTag);

        // Loop through all projectiles and ignore collisions
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != gameObject)  // Don't ignore collision with itself
            {
                Collider2D otherCollider = projectile.GetComponent<Collider2D>();
                if (otherCollider != null)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), otherCollider, true);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    // Check if the object we hit is part of the opposing team
    if ((teamTag == "Team1" && other.CompareTag("Team2")) || (teamTag == "Team2" && other.CompareTag("Team1")))
    {
        // Apply damage to the target
        DealDamage(other);
        if (!isPiersesThoughTarget)
                {
                    Destroy(gameObject); // Destroy projectile if it doesn't pierce through targets
                }
    }
}

    
    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the object we hit is part of the opposing team
        if ((teamTag == "Team1" && other.CompareTag("Team2")) || (teamTag == "Team2" && other.CompareTag("Team1")))
        {
            // Check if it's time to deal damage again
            if (isDamagesMult)
            {
                // Only deal damage if the target hasn't been damaged recently (e.g., within the last 0.5 seconds)
                if (!damageTimers.ContainsKey(other) || Time.time - damageTimers[other] >= 0.1f)
                {
                    damageTimers[other] = Time.time; // Update the timer for this target
                    DealDamage(other);  // Apply damage to the target
                }
            }
        }
    }

    private void DealDamage(Collider2D target)
    {
        // Spawn the particle effect at the collision point
        SpawnParticleEffect(target.transform.position);

        // Check if the object hit has a HealthSystem and apply damage
        HealthSystem health = target.GetComponent<HealthSystem>();
        if (health != null)
        {
            if (isIgnoresArmorArrow)
            {
                health.TakeDamage(damage, isIgnoresArmorArrow);
            }
            else
            {
                health.TakeDamage(damage);
            }
        }
        else
        {
            Debug.LogWarning("HealthSystem not found on the object.");
        }

        // Check for NearestEnemy script and apply stun if applicable
        if (isProjectileStun)
        {
            NearestEnemy nearestEnemy = target.GetComponent<NearestEnemy>();
            if (nearestEnemy != null)
            {
                nearestEnemy.Stun(stunAmount); // Apply the stun
            }
            else
            {
                Debug.LogWarning("NearestEnemy component not found on the object.");
            }
        }
        if (cursesEnemies)
        {
            health.AddCurse(curseAmountP);

        }
    }

    private void SpawnParticleEffect(Vector3 position)
    {
        // Ensure the projectileParticles prefab is assigned
        if (projectileParticles != null)
        {
            // Instantiate the particle effect at the collision point with no rotation
            Instantiate(projectileParticles, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Projectile particle prefab is not assigned.");
        }
    }

    // Optional: Disable the trail after projectile destruction
    private void OnDestroy()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;  // Disable trail effect if needed
        }
    }
}
