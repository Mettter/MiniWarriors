using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;   // Maximum health
    public float currentHealth;     // Current health
    public float armorPoints = 10f; // Armor points
    public bool isTank = false;     // If true, this entity is a tank and will regenerate mana on damage
    public float tankManaAmount = 5f; // The amount of mana added to the currentMana when damage is taken (only for tank)

    public GameObject healthParticles;  // Reference to the health particles prefab
    private Animator animator;
    private ManaSystem manaSystem; // Reference to the ManaSystem script

    private Coroutine bleedCoroutine; // To track and stop an ongoing bleed

    private void Start()
    {
        // Initialize current health to max health
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} spawned with {currentHealth} health and {armorPoints} armor.");

        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component is missing!");
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
    }

    public void TakeDamage(float damageAmount)
    {
        float effectiveDamage = Mathf.Max(damageAmount - armorPoints, 0);
        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} took {damageAmount} damage (effective: {effectiveDamage}). Remaining health: {currentHealth}");

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

        if (currentHealth <= 0)
        {
            Die();
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

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
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
}
