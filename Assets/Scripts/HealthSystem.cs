using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;   // Maximum health
    public float currentHealth;     // Current health
    public float armorPoints = 10f;
    public float maxCursePoints = 10f;
    public float currentCursePoints = 10f; // Armor points
    public bool isTank = false;  
    public bool isInvincibleInvisible = false;    // If true, this entity is a tank and will regenerate mana on damage
    public float tankManaAmount = 5f; 
    public bool staysAfterDeath = false; 
    public float afterlifeAmount = 0f; // The amount of mana added to the currentMana when damage is taken (only for tank)

    [SerializeField] public bool GoesRampage = false;
    [SerializeField] public float RampageHealth = 0;
    [SerializeField] public float YOFFSETVALUE;
    [SerializeField] public GameObject prefab;
    [SerializeField] public bool OverHeals = false; 
    [SerializeField] public int OverHealAmount = 0;

    public bool alreadySpawned = false;

    public GameObject healthParticles;
    public float speedOOfBattle = 3f;  // Reference to the health particles prefab
    private Animator animator;
    private ManaSystem manaSystem; 
    public GameObject ADDBlockParticles;// Reference to the ManaSystem script

    private Coroutine bleedCoroutine; // To track and stop an ongoing bleed

    // New block system
    public float blockCount = 0;  // The number of blocks this entity has available
    public float stunDuration = 2f; // Duration for stun effect
    private bool hasPressedP = false; // Track if P key was pressed at least once
    private float lastPPressTime = -999f; // To store the last time P was pressed
    private float minTimeBetweenPresses = 0f;
    public NearestEnemy nearestEnemy;
    private bool healthMaxed = false; 
    [SerializeField] private bool isPrefab = false;// Minimum time required between P key presses

    public bool isInvincibleAfterTakingDamage = false; // Should the entity be invincible after taking damage?
    public float invincibilityDuration = 1f; // Duration of invincibility frames

    private float invincibilityTimer = 0f;

    private void Start()
    {
        if (healthMaxed == false)// Initialize current health to max health
        {
            maxHealth *= speedOOfBattle * 2;
            currentHealth = maxHealth;

            currentHealth = maxHealth;
            Debug.Log($"{gameObject.name} spawned with {currentHealth} health and {armorPoints} armor.");
            healthMaxed = true;
        }   
        currentCursePoints = 0;
        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing!");
        }

        if (staysAfterDeath == false)
        {
            afterlifeAmount = 0f;
        }

        nearestEnemy = GetComponent<NearestEnemy>();
    if (nearestEnemy == null)
    {
        Debug.LogWarning("NearestEnemy component is missing!");
    }

        // Get the ManaSystem component if this entity is a tank
        if (isTank)
        {
            manaSystem = GetComponent<ManaSystem>(); // Ensure the entity has a ManaSystem
            if (manaSystem == null)
            {
                Debug.LogWarning("ManaSystem component is missing on tank!");
            }
        }
        if(isPrefab)
        {
            hasPressedP = true;  
        }
    }

    private void Update()
    {
        // Check if the P key was pressed and if the time elapsed since the last press is greater than 0.2 seconds
        if (Input.GetKeyDown(KeyCode.P) && Time.time - lastPPressTime >= minTimeBetweenPresses)
        {
            hasPressedP = true;
            lastPPressTime = Time.time; // Update the last press time
            Debug.Log($"{gameObject.name} pressed P key.");
        }

        if (isInvincibleAfterTakingDamage && invincibilityTimer > 0)
    {
        invincibilityTimer -= Time.deltaTime;
    }
        if (currentHealth <= RampageHealth && GoesRampage && !alreadySpawned)
    {
        // Get the parent's tag
        string parentTag = gameObject.tag;

        // Set the spawn position with the YOFFSETVALUE
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + YOFFSETVALUE, transform.position.z);
        
        // Instantiate the prefab at the spawn position
        GameObject spawnedPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        // Set the spawned prefab's tag to match the parent's tag
        spawnedPrefab.tag = parentTag;
        
        // Mark as already spawned
        alreadySpawned = true;
    }
        if(maxCursePoints <= currentCursePoints && staysAfterDeath == false)
        {
            Die(afterlifeAmount);
        }
        else if (maxCursePoints <= currentCursePoints && staysAfterDeath == true)
    {
        if (healthParticles != null)
        {
            Instantiate(healthParticles, transform.position, Quaternion.identity);
        }
        Die(afterlifeAmount);
    }

    }

    public void TakeDamage(float damageAmount, bool isIgnoresArmor = false)
{
    // If invincible, return early and don't apply damage
    if (isInvincibleAfterTakingDamage && invincibilityTimer > 0)
    {
        Debug.Log($"{gameObject.name} is invincible! Damage blocked.");
        return;
    }

    if (isInvincibleInvisible && nearestEnemy.isInvisible)
    {
        return;
    }

    if (!isIgnoresArmor)
    {
        // Block logic
        if (hasPressedP && blockCount > 0)
        {
            // If blockCount is greater than 0 and P key was pressed with sufficient delay, nullify the damage and decrease blockCount
            blockCount--;
            Debug.Log($"{gameObject.name} blocked the damage! Remaining blocks: {blockCount}");

            // Start the block logic (set animator bool to true and stun self)

            // Try to access NearestEnemy and apply stun
            if (nearestEnemy != null)
            {
                Debug.Log($"{gameObject.name} stunned the nearest enemy for {stunDuration} seconds.");
            }

            return; // No damage is taken, exit the function early
        }
        else if (!isPrefab && !hasPressedP)
        {
            Debug.Log($"{gameObject.name} cannot block because P key was not pressed.");
            return; // If P key wasn't pressed, no blocking will happen
        }

        // Calculate effective damage based on armor
        damageAmount = Mathf.Max(damageAmount - armorPoints, 0);
    }

    // Apply damage directly to health
    currentHealth -= damageAmount;
    currentHealth = Mathf.Max(currentHealth, 0);

    Debug.Log($"{gameObject.name} took {damageAmount} damage. Remaining health: {currentHealth}");

    if (animator != null)
    {
        animator.SetBool("isTakingDamage", true);
        Invoke(nameof(ResetTakingDamage), 0.1f);
    }

    if (isTank && manaSystem != null)
    {
        manaSystem.AddMana(tankManaAmount);
        Debug.Log($"{gameObject.name} added {tankManaAmount} mana for being a tank.");
    }

    if (isInvincibleAfterTakingDamage)
    {
        // Start invincibility frames after taking damage
        invincibilityTimer = invincibilityDuration;
        Debug.Log($"{gameObject.name} is now invincible for {invincibilityDuration} seconds.");
    }

    if (currentHealth <= 0 && staysAfterDeath == false)
    {
        Die(afterlifeAmount);
    }
    else if (currentHealth <= 0 && staysAfterDeath == true)
    {
        if (healthParticles != null)
        {
            Instantiate(healthParticles, transform.position, Quaternion.identity);
        }
        Die(afterlifeAmount);
    }
}




    private void ResetTakingDamage()
    {
        if (animator != null)
        {
            animator.SetBool("isTakingDamage", false);
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} healed for {healAmount}. Current health: {currentHealth}");

        if (healthParticles != null)
        {
            Instantiate(healthParticles, transform.position, Quaternion.identity);
        }
    }

    public void AddBlock(float healAmount)
    {
        blockCount += healAmount;

        Debug.Log($"{gameObject.name} healed for {healAmount}. Current health: {currentHealth}");

        if (healthParticles != null)
        {
            Instantiate(ADDBlockParticles, transform.position, Quaternion.identity);
        }
    }
    public void AddCurse(float curseAmount)
    {
        currentCursePoints += curseAmount;
    }

    public void AddArmor(int ArmorAmount, int boostDurationArmor)
    {
        // Add armor to the object
        armorPoints += ArmorAmount;

        // Log armor addition for debugging
        Debug.Log($"{gameObject.name} gained {ArmorAmount} armor. Current armor: {armorPoints}");

        // Start the coroutine to remove the armor after the duration
        StartCoroutine(RemoveArmorAfterDuration(boostDurationArmor, ArmorAmount));
    }

    private IEnumerator RemoveArmorAfterDuration(int duration, float armorAmount)
    {
        // Wait for the duration specified
        yield return new WaitForSeconds(duration);

        // Remove the added armor after the specified time
        armorPoints -= armorAmount;

        // Log the removal of armor for debugging
        Debug.Log($"{gameObject.name} lost {armorAmount} armor. Current armor: {armorPoints}");
    }

    private void Die(float afterlifeAmount)
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject, afterlifeAmount);
    }

    public void RegenerateHealth(float regenAmount, float regenRate)
    {
        StartCoroutine(HealthRegeneration(regenAmount, regenRate));
    }

    private IEnumerator HealthRegeneration(float regenAmount, float regenRate)
    {
        while (currentHealth < maxHealth)
        {
            currentHealth += regenAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            Debug.Log($"{gameObject.name} regenerating health. Current health: {currentHealth}");

            yield return new WaitForSeconds(regenRate);
        }
    }

    /// <summary>
    /// Applies a bleed effect that deals damage over time.
    /// </summary>
    /// <param name="bleedStrength">Damage per tick.</param>
    /// <param name="bleedDuration">Total duration of the bleed effect.</param>
    public void ApplyBleed(float bleedStrength, float bleedDuration)
    {
        // Stop any ongoing bleed before starting a new one
        if (bleedCoroutine != null)
        {
            StopCoroutine(bleedCoroutine);
        }

        // Start the bleed coroutine
        bleedCoroutine = StartCoroutine(BleedRoutine(bleedStrength, bleedDuration));
    }

    private IEnumerator BleedRoutine(float bleedStrength, float bleedDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < bleedDuration)
        {
            // Deal damage every 0.5 seconds
            TakeDamage(bleedStrength);
            Debug.Log($"{gameObject.name} is bleeding! Took {bleedStrength} damage.");

            elapsedTime += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log($"{gameObject.name} has stopped bleeding.");
        bleedCoroutine = null; // Reset the coroutine reference
    }

    // New method to try to find and stun the nearest enemy
    private void TryStunNearestEnemy()
    {
        // Attempt to find the nearest enemy
        NearestEnemy nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            // Apply stun effect to the nearest enemy
            Debug.Log($"{gameObject.name} stunned the nearest enemy for {stunDuration} seconds.");
        }
        else
        {
            Debug.Log("No nearest enemy found to stun.");
        }
    }

    // Try to find the nearest enemy (you may need to adjust this logic depending on your game setup)
    private NearestEnemy FindNearestEnemy()
    {
        // Assuming you have a way to find the nearest enemy in the scene
        // For example, using a simple tag or layer check:
        Collider[] enemies = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Enemy"));

        if (enemies.Length > 0)
        {
            // If there's at least one enemy, return the first one (you can add additional logic to find the nearest)
            return enemies[0].GetComponent<NearestEnemy>();
        }

        return null; // No enemies found
    }

    // Start blocking animation and stun self
    
}
