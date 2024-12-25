using UnityEngine;
using System.Collections; // Required for Coroutines

public class RIFLEMAN : MonoBehaviour
{
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public NearestEnemy healthSystem;  // Reference to the HealthSystem
    public Animator animator;          // Reference to the Animator component
    public float ultimateMana = 100f;  // The amount of mana required to trigger healing
    public int bulletIncreaseCount = 5; // Amount of damage to increase (changed to int)
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
    }

    private void Update()
    {
        // Check if currentMana is equal to or greater than ultimateMana
        if (manaSystem != null && manaSystem.currentMana >= ultimateMana)
        {
            healthSystem.bulletCount += bulletIncreaseCount;
            manaSystem.currentMana = 0;
        }
    }
}
