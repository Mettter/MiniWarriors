using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUEEN_PASSIVE : MonoBehaviour
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
    [SerializeField] private float speedBoostAmount = 1f; // Speed boost amount
    [SerializeField] private float speedBoostDuration = 1f; // Speed boost duration
    [SerializeField] private float healAmount = 10f; // Amount to heal
    [SerializeField] private float healRecharge = 5f; // Time between heals

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = new Color(0.5f, 0f, 0.5f); // Purple color (RGB: 128, 0, 128)

    private List<Collider2D> boostedObjects = new List<Collider2D>(); // To avoid boosting the same object repeatedly
    private bool hasPressedP = false; // Tracks whether the player has pressed the "P" key

    private void Start()
    {
        // Start the healing coroutine, but healing will only happen if "P" is pressed
        StartCoroutine(HealAlliesPeriodically());
    }

    private void Update()
    {
        // Check if "P" key is pressed, and if not already pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            hasPressedP = true; // Set to true when the player presses "P"
            Debug.Log("P key pressed. Boosts will now be applied.");
        }

        if (hasPressedP) // Only apply boosts if the P key has been pressed at least once
        {
            // Detect objects within the boost zone
            Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

            foreach (Collider2D obj in hitObjects)
            {
                if (!boostedObjects.Contains(obj))
                {
                    NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
                    HealthSystem healthSystem = obj.GetComponent<HealthSystem>();

                    // Check if the object's tag matches this object's tag
                    bool isSameTeam = obj.CompareTag(gameObject.tag);

                    if (!isSameTeam)
                    {
                        // Apply range boost (only for opposite-team rangers)
                        if (enemy != null && enemy.isRanger) // Only apply to objects with isRanger = true
                        {
                            enemy.RangeBoost(rangeBoostAmount, rangeBoostDuration); // Range boost logic
                            Debug.Log($"{obj.name} from the opposite team received a range boost.");
                        }

                        // Apply speed boost (only for opposite-team objects)
                        if (enemy != null)
                        {
                            enemy.SpeedBoost(speedBoostAmount, speedBoostDuration); // Speed boost logic
                            Debug.Log($"{obj.name} from the opposite team received a speed boost.");
                        }

                        // Apply armor boost (only for opposite-team objects)
                        if (healthSystem != null)
                        {
                            healthSystem.armorPoints += armorBoostValue;
                            Debug.Log($"{obj.name} received {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
                            StartCoroutine(RemoveArmorBoost(healthSystem, armorBoostValue, boostDuration));
                        }
                    }
                    else
                    {
                        // Apply attack speed boost (only for same-team rangers)
                        if (enemy != null && enemy.isRanger)
                        {
                            enemy.AttackSpeedBoost(attackSpeedBoostAmount, attackSpeedBoostDuration); // Attack speed boost logic
                            Debug.Log($"{obj.name} from the same team received an attack speed boost.");
                        }
                    }

                    // Add object to boosted list
                    boostedObjects.Add(obj);
                    StartCoroutine(RemoveFromBoostedList(obj, boostDuration));
                }
            }
        }
    }

    private IEnumerator HealAlliesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(healRecharge);

            if (hasPressedP) // Only heal if "P" has been pressed
            {
                // Detect objects within the boost zone
                Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

                foreach (Collider2D obj in hitObjects)
                {
                    // Check if the object's tag matches this object's tag
                    if (obj.CompareTag(gameObject.tag))
                    {
                        HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
                        if (healthSystem != null)
                        {
                            healthSystem.Heal(healAmount);
                            Debug.Log($"{obj.name} from the same team was healed by {healAmount}. Current health: {healthSystem.currentHealth}");
                        }
                    }
                }
            }
        }
    }

    private IEnumerator RemoveFromBoostedList(Collider2D obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        boostedObjects.Remove(obj);
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
