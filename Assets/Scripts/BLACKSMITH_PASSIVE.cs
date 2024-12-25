using UnityEngine;
using System.Collections; // Required for Coroutines

public class IncreaseDamageOnUltimateMana : MonoBehaviour
{
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public NearestEnemy healthSystem;  // Reference to the HealthSystem
    public Animator animator;          // Reference to the Animator component
    public float ultimateMana = 100f;  // The amount of mana required to trigger healing
    public int DamageIncreaseAmount = 5; // Amount of damage to increase (changed to int)
    public float delay = 2f;           // Delay in seconds before increasing damage

    private void Start()
    {
        // Automatically assign ManaSystem, HealthSystem, and Animator if not manually assigned
        if (manaSystem == null)
        {
            manaSystem = GetComponent<ManaSystem>();
            if (manaSystem == null)
            {
                Debug.LogWarning("ManaSystem component is missing on the GameObject!");
            }
        }

        if (healthSystem == null)
        {
            healthSystem = GetComponent<NearestEnemy>();
            if (healthSystem == null)
            {
                Debug.LogWarning("NearestEnemy component is missing on the GameObject!");
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component is missing on the GameObject!");
            }
        }
    }

    private void Update()
    {
        // Check if currentMana is equal to or greater than ultimateMana
        if (manaSystem != null && manaSystem.currentMana >= ultimateMana)
        {
            // Set the isUlting boolean in the Animator to true
            if (animator != null)
            {
                animator.SetBool("isUlting", true);
            }
            // Start coroutine to increase damage after a delay
            StartCoroutine(DelayedDamageIncrease());
        }
    }

    private IEnumerator DelayedDamageIncrease()
    {
        Debug.Log($"Delay started. Waiting for {delay} seconds...");
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        IncreaseDamage(); // Call the damage increase logic after the delay

        // Reset the isUlting boolean in the Animator to false after the ultimate finishes
        if (animator != null)
        {
            animator.SetBool("isUlting", false);
        }
    }

    private void IncreaseDamage()
    {
        if (healthSystem != null)
        {
            // Call DamageBoost with the correct type
            healthSystem.DamageBoost(DamageIncreaseAmount, 999); 
            Debug.Log($"Super damage boost triggered! Increased by {DamageIncreaseAmount}.");

            // Reset currentMana to 0 after boosting damage
            manaSystem.currentMana -= ultimateMana;
            Debug.Log("Current mana reset to 0.");
        }
        else
        {
            Debug.LogWarning("HealthSystem component is missing. Cannot boost damage!");
        }
    }
}
