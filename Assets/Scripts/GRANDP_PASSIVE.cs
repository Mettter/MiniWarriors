using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandpaBoostReboot : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostAmount = 2f; // Speed boost amount
    [SerializeField] private float boostDuration = 1f; // Duration of the speed boost
    [SerializeField] private float radius = 3f; // Radius of the boost zone
    [SerializeField] private float yOffset = 1f; // Y offset for the circle's position
    [SerializeField] private float armorBoostValue = 5f; // Armor boost value

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.yellow; // Color of the circle

    private List<Collider2D> boostedObjects = new List<Collider2D>(); // To avoid boosting the same object repeatedly

    private void Update()
    {
        // Detect objects within the boost zone
        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D obj in hitObjects)
        {
            if (!boostedObjects.Contains(obj) && obj.CompareTag(gameObject.tag))
            {
                // Apply speed boost
                NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
                if (enemy != null)
                {
                    enemy.SpeedBoost(boostAmount, boostDuration);
                }

                // Apply armor boost
                HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.armorPoints += armorBoostValue;
                    Debug.Log($"{obj.name} received {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
                    StartCoroutine(RemoveArmorBoost(healthSystem, armorBoostValue, boostDuration));
                }

                // Add object to boosted list
                boostedObjects.Add(obj);
                StartCoroutine(RemoveFromBoostedList(obj, boostDuration));
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
        // Draw the circle to visualize the boost zone
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
} 