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
    [SerializeField] private float boostDelay = 1f; // Delay before the boost applies

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.yellow; // Color of the circle

    private HashSet<Collider2D> boostedObjects = new HashSet<Collider2D>(); // Track boosted objects
    private bool isBoostPending = false; // Flag to track if boost is pending
    private float boostStartTime; // Time when "P" was pressed

    private void Update()
    {
        // Check if "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            isBoostPending = true; // Start the boost delay
            boostStartTime = Time.time; // Store the time when P was pressed
        }

        // Check if enough time has passed since "P" was pressed
        if (isBoostPending && Time.time >= boostStartTime + boostDelay)
        {
            ApplyBoost();
            isBoostPending = false; // Prevent reactivating the boost again
        }
    }

    private void ApplyBoost()
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D obj in hitObjects)
        {
            // Only boost objects that haven't been boosted yet
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
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
