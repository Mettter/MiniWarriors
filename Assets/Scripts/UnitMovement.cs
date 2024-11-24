using System.Collections;
using UnityEngine;

public class NearestEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] public bool isRanger = false;
    [SerializeField] private float originalAttackRange = 1f;
    [SerializeField] private float minimalAttackSpeed = 1f;
    [SerializeField] private bool isAttackSplash = false;
    [SerializeField] private bool targetsFarthestEnemy = false;
    [SerializeField] private bool isGrandma = false;
    [SerializeField] private bool isNotWalking = false;
    //[SerializeField] private bool is = false;
    private float originalAttackSpeed;


    [Header("Bleed Effect Settings")]
    public bool isAttackCauseBleed = false; // Determines if the attack causes bleed
    [Range(1f, 100f)]
    public float bleedStrength = 10f;      // Damage dealt by the bleed effect
    [Range(0.5f, 10f)]
    public float bleedDuration = 3f; 
    private float targetChangeCooldown = 10f; // Time in seconds before switching targets
    private float timeSinceLastTargetChange = 0f; // Track time since last target change
    private Transform currentTarget = null;      // Duration of the bleed effect in seconds



    
    [Header("Mana Settings")]
    [SerializeField] private ManaSystem manaSystem; // Reference to the ManaSystem
    [SerializeField] private float manaPerAttack = 10f; // Mana gained per attack

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float initialProjectileSpeed = 5f;
    [SerializeField] private float speedDecay = 0.5f;
    [SerializeField] private float decayInterval = 0.5f;
    [SerializeField] private float targetOffset = 0.5f;
    [SerializeField] private bool isPiercingProjectile = false;

    [Header("Spawn Positions")]
    [SerializeField] private Transform spawnPosTeam1;
    [SerializeField] private Transform spawnPosTeam2;

    [Header("Support Settings")]
    [SerializeField] private bool isSupport = false;

    private Transform nearestEnemy;
    private bool isAttacking = false;
    private bool canMove = false;
    private Animator animator;
    public bool isStunned = false; // Determines if the character is stunned
    private float stunDuration = 0f; 
    private bool pKeyPressed = false;
    private bool isShooting = false;

    [Header("Particle Effects")]
    [SerializeField] private GameObject damageBoostP; // Particle effect for damage boost
    [SerializeField] private GameObject speedBoostP;  // Particle effect for speed boost
    [SerializeField] private GameObject rangeBoostP; 
    [SerializeField] public float effectSpawnYOffset;


    private Coroutine currentRangeBoostCoroutine;

    private void Start()
    {
        float originalAttackSpeed = attackCooldown;
        originalAttackRange = attackRange;
        animator = GetComponent<Animator>();

        // Automatically assign ManaSystem if not manually set
        if (manaSystem == null)
        {
            manaSystem = GetComponent<ManaSystem>();
            if (manaSystem == null)
            {
                Debug.LogWarning("ManaSystem component is missing on the GameObject!");
            }
        }
    }

    private void Update()
{
    void OnDestroy()
    {
        if (projectilePrefab != null)
        {
            Destroy(projectilePrefab); // Automatically destroy the projectilePrefab
            Debug.Log("ProjectilePrefab was destroyed during OnDestroy.");
        }
    }
    
    if (!canMove && Input.GetKeyDown(KeyCode.P))
    {
        canMove = true;
        Debug.Log("Movement enabled!");
    }

    if (isStunned)
        {
            stunDuration -= Time.deltaTime;
            animator.SetBool("isWalking", false);
            if (stunDuration <= 0f)
            {
                isStunned = false; // Stop being stunned when duration ends
            }
        }

    if (isStunned) return;

    timeSinceLastTargetChange += Time.deltaTime;

    // Prioritize farthest enemy target if needed, but only if cooldown has passed
    timeSinceLastTargetChange += Time.deltaTime;

    // Если цель еще не установлена, найдем ее (только если прошло достаточно времени)
    if (currentTarget == null && timeSinceLastTargetChange >= targetChangeCooldown)
    {
        if (targetsFarthestEnemy)
        {
            FindFarthestObject();
        }
        else
        {
            FindNearestObject();
        }
    }

    if (true)
    {
        if (isSupport && targetsFarthestEnemy == false)
        {
            FindNearestAlly(); // Find the nearest ally if isSupport is true
        }
        
        else if  (targetsFarthestEnemy == false)
        {
            FindNearestObject(); // Otherwise, find the nearest enemy
        }
    }

    if (true)
    {
        FlipSprite();
        if (attackCooldown < minimalAttackSpeed)    
            attackCooldown = minimalAttackSpeed;

        if (isNotWalking)    
            moveSpeed = 0;
    }

    if (nearestEnemy == null)
{
    if (isSupport)
    {
        FindNearestAlly(); // Find the nearest ally if isSupport is true
    }
    else
    {
        FindNearestObject(); // Otherwise, find the nearest enemy
    }
}

    if (isStunned) return;

    if (!canMove) return;

    if (nearestEnemy == null)
    {
        animator.SetBool("isAttacking", false);
    }

    if (nearestEnemy != null && !isAttacking)
    {
        float distance = Vector2.Distance(transform.position, nearestEnemy.position);

        if (isSupport)
    {
        // Move towards the ally only if outside attack range
        if (distance > attackRange)
        {
            MoveTowards(nearestEnemy.position);
            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            // Stop walking when within attack range
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
    else
    {
        // Attacker behavior (Ranger or Melee)
        if (distance <= attackRange && isRanger)
        {
            StartCoroutine(ShootProjectile());
        }
        else if (distance > attackRange)
        {
            // Move towards the enemy if not in attack range
            MoveTowards(nearestEnemy.position);

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }
        else if (!isRanger && distance <= attackRange)
        {
            StartCoroutine(Attack());
        }
    }
}
else
{
    // If nearestEnemy is null, stop walking and ensure no movement towards null target
    if (animator != null)
    {
        animator.SetBool("isWalking", false);
    }
    }
}

    public void ResetTarget()
    {
        nearestEnemy = null;  // Reset the nearest enemy Transform
  // Reset the nearest ally Transform
    }

    public void SpeedBoost(float boostValue, float boostDuration)
    {
        StartCoroutine(SpeedBoostRoutine(boostValue, boostDuration));
    }

    private IEnumerator SpeedBoostRoutine(float boostValue, float boostDuration)
    {
        // Spawn the speed boost particles at the local Y offset
        if (speedBoostP != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + effectSpawnYOffset, transform.position.z);
            Instantiate(speedBoostP, spawnPosition, Quaternion.identity);
        }

        float originalSpeed = moveSpeed;
        moveSpeed += boostValue;

        Debug.Log($"{gameObject.name} speed boosted by {boostValue}. Current speed: {moveSpeed}. Duration: {boostDuration} seconds.");

        yield return new WaitForSeconds(boostDuration);

        moveSpeed = originalSpeed;

        Debug.Log($"{gameObject.name} speed boost ended. Speed reset to {moveSpeed}.");
    }

private void FindNearestAlly()
{
    string targetTag = gameObject.CompareTag("Team1") ? "Team1" : "Team2";
    GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
    float shortestDistance = Mathf.Infinity;
    GameObject closestObject = null;

    foreach (GameObject obj in objects)
    {
        // Skip the object if it's the same as the one this script is attached to
        if (obj == gameObject) continue;

        // Skip objects that have the Projectile script
        if (obj.GetComponent<Projectile>() != null) continue;

        // Get the NearestEnemy component of the potential ally
        NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
        
        if (enemy != null)
        {
            // If the current object (this script) is Grandma, only consider enemies with isRanger = true
            if (gameObject.GetComponent<NearestEnemy>().isGrandma && !enemy.isRanger) continue;
        }

        // Calculate the distance
        float distance = Vector2.Distance(transform.position, obj.transform.position);

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            closestObject = obj;
        }
    }

    if (closestObject != null)
    {
        nearestEnemy = closestObject.transform; // Now the nearest ally is correctly assigned
    }
}



    public void Stun(float duration)
    {
        isStunned = true;  // Set the character to be stunned
        stunDuration = duration; // Set the stun duration
    }

    public void DamageBoost(int boostValue, float boostDuration)
    {
        StartCoroutine(DamageBoostRoutine(boostValue, boostDuration));
    }

    private IEnumerator DamageBoostRoutine(int boostValue, float boostDuration)
    {
        // Spawn the damage boost particles at the local Y offset
        if (damageBoostP != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + effectSpawnYOffset, transform.position.z);
            Instantiate(damageBoostP, spawnPosition, Quaternion.identity);
        }

        int originalAttackDamage = attackDamage;
        attackDamage += boostValue;

        Debug.Log($"{gameObject.name} damage boosted by {boostValue}. Current attack damage: {attackDamage}. Duration: {boostDuration} seconds.");

        yield return new WaitForSeconds(boostDuration);

        attackDamage = originalAttackDamage;

        Debug.Log($"{gameObject.name} damage boost ended. Attack damage reset to {attackDamage}.");
    }

    public void RangeBoost(float boostValue, float boostDuration)
    {
        // Spawn the range boost particles at the local Y offset
        if (rangeBoostP != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + effectSpawnYOffset, transform.position.z);
            Instantiate(rangeBoostP, spawnPosition, Quaternion.identity);
        }

        // Prevent stacking by stopping any active range boost coroutine
        if (currentRangeBoostCoroutine != null)
        {
            StopCoroutine(currentRangeBoostCoroutine);
        }

        // Start the new range boost coroutine
        currentRangeBoostCoroutine = StartCoroutine(ApplyRangeBoost(boostValue, boostDuration));
    }

    private IEnumerator ApplyRangeBoost(float boostValue, float boostDuration)
    {
        attackRange += boostValue; // Increase attack range
        yield return new WaitForSeconds(boostDuration); // Wait for the boost duration
        attackRange = originalAttackRange; // Reset to the original attack range
        currentRangeBoostCoroutine = null; // Clear the boost reference
    }

    public void AttackSpeedBoost(float sPboostValue, float sPboostDuration)
    {
        // Save the original attack speed to revert back after boost
        float originalAttackSpeed = attackCooldown;

        // Apply the boost
        attackCooldown -= sPboostValue;

        // Start a coroutine to reset the attack speed after the boost duration
        StartCoroutine(ResetAttackSpeed(sPboostDuration));
    }

    // Coroutine to reset attack speed after a delay
    private IEnumerator ResetAttackSpeed(float sPboostDuration)
    {
        yield return new WaitForSeconds(sPboostDuration);

        // Restore the original attack speed
        attackCooldown = originalAttackSpeed;
    }


    private void FindNearestObject()
{
    string targetTag = isSupport ? gameObject.tag : (gameObject.CompareTag("Team1") ? "Team2" : "Team1");
    GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
    float shortestDistance = Mathf.Infinity;
    GameObject closestObject = null;

    foreach (GameObject obj in objects)
    {
        // Check if the object has a Projectile script
        if (obj.GetComponent<Projectile>() != null)
        {
            continue;  // Skip this object if it has the Projectile script
        }

        float distance = Vector2.Distance(transform.position, obj.transform.position);

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            closestObject = obj;
        }
    }

    if (closestObject != null)
    {
        nearestEnemy = closestObject.transform;
    }
}

    private void FindFarthestObject()
{
    string targetTag = isSupport ? gameObject.tag : (gameObject.CompareTag("Team1") ? "Team2" : "Team1");
    GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
    float farthestDistance = 0f; // Start with the smallest possible distance
    GameObject farthestObject = null;

    foreach (GameObject obj in objects)
    {
        // Check if the object has a Projectile script
        if (obj.GetComponent<Projectile>() != null)
        {
            continue;  // Skip this object if it has the Projectile script
        }

        float distance = Vector2.Distance(transform.position, obj.transform.position);

        if (distance > farthestDistance)
        {
            farthestDistance = distance;
            farthestObject = obj;
        }
    }

    if (farthestObject != null)
    {
        nearestEnemy = farthestObject.transform; // Assign the farthest object's transform
    }
}



    private void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    private void FlipSprite()
    {
        if (nearestEnemy == null) return;

        Vector2 directionToEnemy = nearestEnemy.position - transform.position;

        if (directionToEnemy.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (directionToEnemy.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private IEnumerator Attack()
{
    // Determine the opposite team's count based on the current game object's tag
    int oppositeTeamCount = gameObject.CompareTag("Team1") ? TeamManager.Team2Count : TeamManager.Team1Count;

    // If the opposite team's count is 0, do not proceed with the attack

    // Set attacking state
    isAttacking = true;

    if (animator != null)
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
    }

    yield return new WaitForSeconds(attackDelay);

    Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange);

    // Check if there are any enemies in range from the opposite team
    bool hasValidTarget = false;

    // Check if there are any valid enemies to attack
    foreach (Collider2D enemy in hitEnemies)
    {
        if (enemy != null && enemy.CompareTag(gameObject.CompareTag("Team1") ? "Team2" : "Team1"))
        {
            hasValidTarget = true;
            break; // Exit loop early if a valid enemy is found
        }
    }

    // If no valid targets found, do not attack
    if (!hasValidTarget)
    {
        Debug.Log("No valid enemies in range to attack.");
        isAttacking = false; // Reset attacking state
        yield break; // Exit the attack coroutine
    }

    // Proceed with the attack since there are valid enemies
    if (isAttackSplash) // Attack all enemies within the range
    {
        foreach (Collider2D enemy in hitEnemies)
        {
            // Ensure the enemy is still valid and belongs to the opposite team
            if (enemy != null && enemy.CompareTag(gameObject.CompareTag("Team1") ? "Team2" : "Team1"))
            {
                HealthSystem health = enemy.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name} attacked {enemy.name} for {attackDamage} damage.");

                    // Apply bleed if conditions are met
                    if (isAttackCauseBleed && !isRanger)
                    {
                        health.ApplyBleed(bleedStrength, bleedDuration);
                        Debug.Log($"{enemy.name} is now bleeding for {bleedDuration} seconds!");
                    }
                }
            }
        }
    }
    else // Attack only the nearest enemy
    {
        Collider2D nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;  // Start with an infinitely large distance

        foreach (Collider2D enemy in hitEnemies)
        {
            // Ensure the enemy is still valid and belongs to the opposite team
            if (enemy != null && enemy.CompareTag(gameObject.CompareTag("Team1") ? "Team2" : "Team1"))
            {
                float distanceToEnemy = Vector2.Distance(attackPosition, enemy.transform.position);

                // Check if this enemy is the nearest
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;  // Set the nearest enemy
                }
            }
        }

        if (nearestEnemy != null)
        {
            HealthSystem health = nearestEnemy.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} attacked {nearestEnemy.name} for {attackDamage} damage.");

                // Apply bleed if conditions are met
                if (isAttackCauseBleed && !isRanger)
                {
                    health.ApplyBleed(bleedStrength, bleedDuration);
                    Debug.Log($"{nearestEnemy.name} is now bleeding for {bleedDuration} seconds!");
                }
            }
        }
    }

    GainMana();
    yield return new WaitForSeconds(attackCooldown);

    // Reset attack state after cooldown
    isAttacking = false;
}






   private IEnumerator MoveProjectile(GameObject projectile)
{
    float currentSpeed = initialProjectileSpeed;
    Transform projectileTransform = projectile.transform;
    SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();

    // Initial target position with offset
    Vector3 targetPosition = nearestEnemy.position + new Vector3(0, targetOffset, 0);

    while (projectile != null)
    {
        // Check if the target is still valid (alive and exists)
        if (nearestEnemy == null || nearestEnemy.gameObject == null)
        {
            FindNearestObject(); // Find a new target if the current one is invalid
            if (nearestEnemy == null || nearestEnemy.gameObject == null)
            {
                // If no target is found, destroy the projectile and stop the coroutine
                Destroy(projectile);
                yield break;
            }

            // Update the target position if a new valid target is found
            targetPosition = nearestEnemy.position + new Vector3(0, targetOffset, 0);
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

        // Check if the projectile is close enough to the target to destroy it (non-piercing)
        if (!isPiercingProjectile && Vector3.Distance(projectileTransform.position, targetPosition) < 0.1f)
        {
            // Destroy the projectile if it's a non-piercing projectile and reaches the target
            Destroy(projectile);
            break;
        }

        // If speed reaches zero, stop the projectile
        if (currentSpeed <= 0f)
        {
            Debug.Log("Projectile stopped due to speed decay.");
            Destroy(projectile);
            break;
        }
    }
}



// Assuming you have an Animator component and isAttacking as a boolean parameter
private bool isCurrentlyAttacking = false; // This tracks if the object is currently attacking

private IEnumerator ShootProjectile()
{
    // Prevent multiple coroutines from running at the same time
    if (isCurrentlyAttacking)
        yield break;

    isCurrentlyAttacking = true; // Mark that shooting is in progress

    // Set the isAttacking animator bool to true at the start of the coroutine
    if (animator != null)
    {
        animator.SetBool("isAttacking", true); // Start the attack animation
        animator.SetBool("isWalking", false); // Optional: stop walking animation if needed
    }

    // Set the position from where the projectile will spawn
    Vector3 spawnPosition = transform.position;

    // Adjust spawn position based on team-specific spawn locations
    if (gameObject.CompareTag("Team1") && spawnPosTeam1 != null)
    {
        spawnPosition = spawnPosTeam1.position;
    }
    else if (gameObject.CompareTag("Team2") && spawnPosTeam2 != null)
    {
        spawnPosition = spawnPosTeam2.position;
    }

    // Wait for the attack cooldown before shooting the projectile
    yield return new WaitForSeconds(attackCooldown);

    // Instantiate the projectile at the spawn position
    GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    projectile.tag = gameObject.tag; // Ensure the projectile has the same tag as the unit

    // Start moving the projectile
    StartCoroutine(MoveProjectile(projectile));

    // Handle mana and other cooldown-related mechanics (if needed)
    GainMana();

    // Wait for the attack cooldown delay after the shot before allowing another shot
    yield return new WaitForSeconds(attackCooldown);

    // Reset attack animation and allow it to continue only if this object is attacking
    if (animator != null && !isCurrentlyAttacking)
    {
        animator.SetBool("isAttacking", false); // Stop attack animation
    }

    isCurrentlyAttacking = false; // Mark that shooting is finished, allowing for the next attack
}






    private void GainMana()
    {
        if (manaSystem != null)
        {
            manaSystem.currentMana += manaPerAttack;
            Debug.Log($"{gameObject.name} gained {manaPerAttack} mana. Current Mana: {manaSystem.currentMana}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}
