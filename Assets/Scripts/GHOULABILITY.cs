using System.Collections;
using UnityEngine;

public class GHOULABILITY : MonoBehaviour
{
    public float healthRegen = 2f; // Time interval for health regeneration
    public float healAmount = 10f; // Initial amount to heal each time
    public float healAmountIncrease = 5f; // Amount by which to increase healAmount each time
    public float speedUpAmount = 0.2f; // Amount to add to the attack speed (increase cooldown)
    public float minAttackSpeed = 0.2f; // Minimum attack speed (maximum cooldown time)
    public float maxAttackCooldown = 2f; // Maximum attack cooldown value (slowest possible attack rate)

    private HealthSystem healthSystem;
    private NearestEnemy nearestEnemy;

    private bool isDetectionStarted = false; // Flag to track if detection has started

    private void Start()
    {
        // Get the HealthSystem and NearestEnemy components from the host object (self)
        healthSystem = GetComponent<HealthSystem>();
        nearestEnemy = GetComponent<NearestEnemy>();

        // Check if the HealthSystem is attached
        if (healthSystem == null)
        {
            Debug.LogWarning("HealthSystem not found on this object.");
        }

        // Check if the NearestEnemy is attached
        if (nearestEnemy == null)
        {
            Debug.LogWarning("NearestEnemy not found on this object.");
        }
    }

    private void Update()
    {
        // Start the functionality when the 'P' key is pressed
        if (Input.GetKeyDown(KeyCode.P) && !isDetectionStarted)
        {
            isDetectionStarted = true;
            StartCoroutine(HealthRegenAndSpeedUpCoroutine());
        }
    }

    private IEnumerator HealthRegenAndSpeedUpCoroutine()
    {
        // Continuously heal and adjust attack speed and cooldown at the specified intervals
        while (isDetectionStarted)
        {
            yield return new WaitForSeconds(healthRegen);

            if (healthSystem != null)
            {
                // Heal the host object by the specified amount
                healthSystem.Heal(healAmount);
                Debug.Log("Healed " + healAmount + " HP");

                // Increase the heal amount after each iteration
                healAmount += healAmountIncrease;
                Debug.Log("Increased healAmount: " + healAmount);
            }

            if (nearestEnemy != null)
            {
                // Check if the NearestEnemy's minimalAttackSpeed exists and adjust attack speed (as cooldown)
                float currentAttackSpeed = nearestEnemy.minimalAttackSpeed;

                // Only decrease attack speed (increase cooldown) if the attack speed is greater than the minimum
                if (currentAttackSpeed > minAttackSpeed)
                {
                    // Decrease attack speed (this increases the cooldown time)
                    currentAttackSpeed -= speedUpAmount;

                    // Ensure it doesn't go below the minimum attack speed
                    if (currentAttackSpeed < minAttackSpeed)
                    {
                        currentAttackSpeed = minAttackSpeed;
                    }

                    // Update the NearestEnemy's minimalAttackSpeed (which is acting as cooldown)
                    nearestEnemy.minimalAttackSpeed = currentAttackSpeed;

                    Debug.Log("Updated attack speed (increased cooldown): " + currentAttackSpeed);
                }
                else
                {
                    Debug.Log("Attack speed is already at or below " + minAttackSpeed + ", no further increase.");
                }

                // Check if the NearestEnemy's attackCooldown exists and adjust it (increase cooldown)
                float currentAttackCooldown = nearestEnemy.attackCooldown;

                // Only increase the attackCooldown if it's less than the maximum allowed value
                if (currentAttackCooldown < maxAttackCooldown)
                {
                    // Increase the attack cooldown by the same speedUpAmount
                    currentAttackCooldown += speedUpAmount;

                    // Ensure it doesn't exceed the maximum attack cooldown
                    if (currentAttackCooldown > maxAttackCooldown)
                    {
                        currentAttackCooldown = maxAttackCooldown;
                    }

                    // Update the NearestEnemy's attackCooldown
                    nearestEnemy.attackCooldown = currentAttackCooldown;

                    Debug.Log("Updated attack cooldown (slowed down): " + currentAttackCooldown);
                }
                else
                {
                    Debug.Log("Attack cooldown is already at or above " + maxAttackCooldown + ", no further increase.");
                }
            }
        }
    }
}
