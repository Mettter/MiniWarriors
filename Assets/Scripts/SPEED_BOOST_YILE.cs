using System.Collections;
using UnityEngine;

public class Grandpa : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostAmount = 2f; // Speed boost amount
    [SerializeField] private float boostDuration = 1f; // Duration of the speed boost
    [SerializeField] private float radius = 3f; // Radius of the boost zone
    [SerializeField] private float yOffset = 1f; // Y offset for the circle's position

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = Color.yellow; // Color of the circle

    private Collider2D boostedObject; // Tracks the currently boosted object
    private bool boostActivated = false; // Tracks whether boosting is allowed

    private void Update()
    {
        // Check if the P key was pressed to enable boosting
        if (Input.GetKeyDown(KeyCode.P))
        {
            boostActivated = true;
            Debug.Log("Boosting activated! Pressed P.");
        }

        // Only apply boosting if activation has occurred
        if (!boostActivated) return;

        // Detect objects within the boost zone
        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        float closestDistance = float.MaxValue;
        Collider2D nearestObject = null;

        foreach (Collider2D obj in hitObjects)
        {
            NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
            if (enemy != null && !IsAlreadyBoosted(obj))
            {
                float distance = Vector2.Distance(center, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestObject = obj;
                }
            }
        }

        // Apply speed boost to the nearest object
        if (nearestObject != null)
        {
            NearestEnemy enemy = nearestObject.GetComponent<NearestEnemy>();
            if (enemy != null)
            {
                enemy.SpeedBoost(boostAmount, boostDuration);
                Debug.Log($"{nearestObject.name} received a speed boost of {boostAmount} for {boostDuration} seconds.");
                boostedObject = nearestObject;
                StartCoroutine(RemoveBoostAfterDuration(boostDuration));
            }
        }
    }

    private bool IsAlreadyBoosted(Collider2D obj)
    {
        return boostedObject == obj;
    }

    private IEnumerator RemoveBoostAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        boostedObject = null; // Allow the object to be boosted again
    }

    private void OnDrawGizmos()
    {
        // Draw the circle to visualize the boost zone
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
