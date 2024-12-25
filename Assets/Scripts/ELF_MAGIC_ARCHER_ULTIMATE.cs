using System.Collections;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // Prefab to instantiate as a projectile
    [SerializeField] private Transform spawnPosition;    // Reference to the spawn position (Transform)
    [SerializeField] private float attackCooldown = 1f;   // Cooldown time between attacks
    [SerializeField] private float initialProjectileSpeed = 5f; // Initial speed of the projectile
    [SerializeField] private float speedDecay = 0.5f;     // Rate of speed decay
    [SerializeField] private float decayInterval = 0.1f;  // Interval between each speed decay step
    [SerializeField] private bool isPiercingProjectile = false; // Whether the projectile can pierce through enemies
    [SerializeField] private float targetOffset = 0.5f;   // Vertical offset for targeting enemies
    [SerializeField] private float yOffset = 0f;          // Vertical offset for projectile spawn position (if not created already)
    [SerializeField] private float xOffset = 0f;          // Horizontal offset for projectile spawn position
    [SerializeField] private bool shootsProjectile = true; // Whether the projectile should be shot or just spawned
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public float ultimateMana = 100f;   // Mana cost for ultimate ability

    private bool isCurrentlyAttacking = false; // Tracks if the object is currently attacking
    private Transform targetEnemy; // Reference to the farthest enemy
    private bool hasPressedP = false; // Track if P key has been pressed at least once
    private NearestEnemy nearestEnemyScript; // Reference to the NearestEnemy script of the host

    private void Start()
    {
        if (manaSystem == null)
        {
            manaSystem = GetComponent<ManaSystem>();
            if (manaSystem == null)
            {
                Debug.LogWarning("ManaSystem component is missing on the GameObject!");
            }
        }

        // Get the NearestEnemy script from the host
        nearestEnemyScript = GetComponent<NearestEnemy>();
        if (nearestEnemyScript == null)
        {
            Debug.LogWarning("NearestEnemy script is missing on the GameObject!");
        }
    }

    private void Update()
    {
        // Check for the P key press to enable projectile shooting
        if (Input.GetKeyDown(KeyCode.P))
        {
            hasPressedP = true;
        }

        // Only shoot if P key has been pressed and enough mana is available
        if (hasPressedP && manaSystem != null && manaSystem.currentMana >= ultimateMana)
        {
            StartCoroutine(ShootProjectile());
        }
    }

    private IEnumerator MoveProjectile(GameObject projectile, Vector3 spawnPosition)
    {
        float currentSpeed = initialProjectileSpeed;
        Transform projectileTransform = projectile.transform;
        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();

        // Set the initial spawn position as the start point for the projectile
        projectileTransform.position = spawnPosition;

        // Target position based on the farthest enemy
        Vector3 targetPosition = targetEnemy != null
            ? targetEnemy.position + new Vector3(0, targetOffset, 0)
            : Vector3.zero;

        while (projectile != null)
        {
            // Check if the target is still valid (alive and exists)
            if (targetEnemy == null || targetEnemy.gameObject == null)
            {
                FindTarget(); // Find a new target if the current one is invalid
                if (targetEnemy == null || targetEnemy.gameObject == null)
                {
                    yield break;
                }

                targetPosition = targetEnemy.position + new Vector3(0, targetOffset, 0);
            }

            // Calculate direction towards the target
            Vector3 direction = (targetPosition - projectileTransform.position).normalized;

            // Move the projectile
            projectileTransform.position += direction * currentSpeed * Time.deltaTime;

            // Flip the sprite based on movement direction
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // Apply speed decay
            yield return new WaitForSeconds(decayInterval);
            currentSpeed = Mathf.Max(0, currentSpeed - speedDecay);

            // Destroy the projectile if it reaches the target (and it's not piercing)
            if (!isPiercingProjectile && Vector3.Distance(projectileTransform.position, targetPosition) < 0.1f)
            {
                Destroy(projectile);
                break;
            }
        }
    }

    private IEnumerator ShootProjectile()
    {
        if (isCurrentlyAttacking)
            yield break;

        // Deduct ultimate mana instead of setting current mana to 0
        manaSystem.currentMana = 0;
        isCurrentlyAttacking = true;

        // Calculate the spawn position with both x and y offsets right before shooting
        Vector3 spawnPosition = this.spawnPosition != null ? this.spawnPosition.position + new Vector3(xOffset, yOffset, 0) : transform.position + new Vector3(xOffset, yOffset, 0);

        FindTarget(); // Find the farthest valid target

        // Ensure the target is still valid before instantiating
        if (targetEnemy == null || targetEnemy.gameObject == null)
        {
            isCurrentlyAttacking = false;
            yield break; // Exit if no valid target is found
        }

        yield return new WaitForSeconds(attackCooldown);

        if (shootsProjectile)
        {
            // Instantiate the projectile at the spawn position (not at the target position)
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.tag = gameObject.tag; // Assign the shooter's tag to the projectile

            // Call the MoveProjectile to handle the movement towards the target
            StartCoroutine(MoveProjectile(projectile, spawnPosition));
        }
        else
        {
            // Simply spawn the object on top of the host without projectile mechanics
            GameObject spawnedObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            spawnedObject.tag = gameObject.tag;
        }

        yield return new WaitForSeconds(attackCooldown);

        isCurrentlyAttacking = false;
    }

    private void ApplyStun(float stunDuration)
    {
        // Apply stun logic here
        Debug.Log("Stunned for " + stunDuration + " seconds.");
    }

    private void FindTarget()
    {
        // Get the opposite team tag
        string targetTag = gameObject.CompareTag("Team1") ? "Team2" : "Team1";

        // Find all objects with the opposite tag
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);

        float farthestDistance = -Mathf.Infinity;
        targetEnemy = null;

        foreach (GameObject target in potentialTargets)
        {
            // Skip objects with a Projectile component
            if (target.GetComponent<Projectile>() != null) continue;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                targetEnemy = target.transform;
            }
        }
    }

    // Detect collision and destroy the projectile
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the projectile collides with a target enemy
        if (other.CompareTag(targetEnemy.tag))
        {
            //Destroy(gameObject); // Destroy the projectile upon collision with the target
        }
    }
}
