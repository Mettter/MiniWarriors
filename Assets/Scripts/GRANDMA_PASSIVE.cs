using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float rangeBoostAmount = 2f; // Range boost amount
    [SerializeField] private float attackSpeedBoostAmount = 0.5f; // Attack speed boost amount (lower value means faster attacks)
    [SerializeField] private float attackSpeedBoostDuration = 1f; // Attack speed boost duration
    [SerializeField] private float rangeBoostDuration = 1f; // Duration of the range boost
    [SerializeField] private float boostDuration = 1f; // Duration of the general boost
    [SerializeField] private float radius = 3f; // Radius of the boost zone
    [SerializeField] private float yOffset = 1f; // Y offset for the circle's position
    [SerializeField] private float armorBoostValue = 5f; // Armor boost value

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = new Color(0.5f, 0f, 0.5f); // Purple color (RGB: 128, 0, 128)

    private bool hasPressedP = false; // Tracks whether the player has pressed the "P" key

    private void Start()
    {
        // Start the coroutine to check for targets every second
        StartCoroutine(SeekTargets());
    }

    private void Update()
    {
        // Check if "P" key is pressed, and if not already pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            hasPressedP = true; // Set to true when the player presses "P"
            Debug.Log("P key pressed. Attack Speed Boost will now be applied.");
        }
    }

    private IEnumerator SeekTargets()
    {
        while (true)
        {
            ApplyBoosts();
            yield return new WaitForSeconds(1f); // Wait for 1 second before seeking again
        }
    }

    private void ApplyBoosts()
    {
        // Detect objects within the boost zone
        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag(gameObject.tag))
            {
                // Apply range boost (always applies, no P key check)
                NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
                if (enemy != null && enemy.isRanger) // Only apply to objects with isRanger = true
                {
                    enemy.RangeBoost(rangeBoostAmount, rangeBoostDuration); // Range boost logic
                }

                // Apply attack speed boost only if P key has been pressed
                if (enemy != null && enemy.isRanger && hasPressedP) // Only apply if P has been pressed
                {
                    enemy.AttackSpeedBoost(attackSpeedBoostAmount, attackSpeedBoostDuration);
                }

                // Apply armor boost (always applies, as this is not conditionally based on isRanger)
                HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.armorPoints += armorBoostValue;
                    Debug.Log($"{obj.name} received {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
                    StartCoroutine(RemoveArmorBoost(healthSystem, armorBoostValue, boostDuration));
                }
            }
        }
    }

    private IEnumerator RemoveArmorBoost(HealthSystem healthSystem, float armorBoostValue, float duration)
    {
        yield return new WaitForSeconds(duration);
        healthSystem.armorPoints -= armorBoostValue;
        Debug.Log($"{healthSystem.gameObject.name} lost {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
    }

    private void OnDrawGizmos()
    {
        // Draw the circle to visualize the boost zone with purple color
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
