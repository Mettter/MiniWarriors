using UnityEngine;

public class HealOnUltimateMana : MonoBehaviour
{
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public HealthSystem healthSystem;  // Reference to the HealthSystem
    public float ultimateMana = 100f;  // The amount of mana required to trigger healing
    public float superRegen = 50f;     // Amount of health to regenerate

    private void Start()
    {
        // Automatically assign ManaSystem and HealthSystem if not manually assigned
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
            healthSystem = GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                Debug.LogWarning("HealthSystem component is missing on the GameObject!");
            }
        }
    }

    private void Update()
    {
        // Check if currentMana is equal to or greater than ultimateMana
        if (manaSystem != null && manaSystem.currentMana >= ultimateMana)
        {
            // Heal the entity if the condition is met
            HealEntity();
        }
    }

    private void HealEntity()
    {
        if (healthSystem != null)
        {
            healthSystem.Heal(superRegen); // Heal for superRegen amount
            Debug.Log($"Super regen triggered! Healed for {superRegen} health.");

            // Reset currentMana to 0 after healing
            manaSystem.currentMana = 0f;
            Debug.Log("Current mana reset to 0.");
        }
        else
        {
            Debug.LogWarning("HealthSystem component is missing. Cannot heal!");
        }
    }
}
