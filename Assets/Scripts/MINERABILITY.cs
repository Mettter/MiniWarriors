using System.Collections;
using UnityEngine;

public class MinerAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private int buffPercent = 10; // Buff percentage for damage boost
    [SerializeField] private float buffDuration = 1f; // Duration of the buff
    [SerializeField] private float detectionRadius = 5f; // Radius to search for enemies
    [SerializeField] private float checkInterval = 0.5f; // How often it checks for enemies

    [Header("Team Settings")]
    [SerializeField] private string hostTeam = "Team1"; // Manually set host team
    [SerializeField] private string enemyTeam = "Team2"; // Manually set enemy team

    private NearestEnemy hostEnemy;

    private void Start()
    {
        // Get the NearestEnemy script from the host
        hostEnemy = GetComponent<NearestEnemy>();

        if (hostEnemy == null)
        {
            Debug.LogError("MinerAbility: NearestEnemy script not found on the host!");
            return;
        }

        // Start continuously checking for enemies
        StartCoroutine(CheckForEnemies());
    }

    private IEnumerator CheckForEnemies()
    {
        while (true) // Keeps running infinitely
        {
            GameObject nearestTarget = FindNearestValidTarget();

            if (nearestTarget != null)
            {
                HealthSystem targetHealth = nearestTarget.GetComponent<HealthSystem>();

                if (targetHealth != null)
                {
                    int boostValue = Mathf.RoundToInt(targetHealth.maxHealth * (buffPercent / 100f));
                    
                    // Apply damage boost to host
                    hostEnemy.DamageBoost(boostValue, buffDuration);
                    Debug.Log($"{gameObject.name} gained {boostValue} damage boost for {buffDuration} seconds!");
                }
            }

            yield return new WaitForSeconds(checkInterval); // Check again after some time
        }
    }

    private GameObject FindNearestValidTarget()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        GameObject nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in hitObjects)
        {
            GameObject obj = col.gameObject;

            // Ensure the object is in the enemy team and not a projectile
            if (obj.CompareTag(enemyTeam) && obj.GetComponent<Projectile>() == null)
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestTarget = obj;
                }
            }
        }

        return nearestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius in Unity Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}