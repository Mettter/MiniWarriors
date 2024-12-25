using System.Collections;
using UnityEngine;

public class E : MonoBehaviour
{
    [Header("Healing Settings")]
    [SerializeField] private float healAmount = 10f; // Amount to heal
    [SerializeField] private float healRecharge = 5f; // Time between heals
    [SerializeField] private float radius = 3f; // Radius of the healing zone
    [SerializeField] private float yOffset = 1f; // Y offset for the circle's position

    private void Start()
    {
        // Start the healing coroutine
        StartCoroutine(HealNearestAllyPeriodically());
    }

    private IEnumerator HealNearestAllyPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(healRecharge);

            // Detect objects within the healing zone
            Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

            float closestDistance = float.MaxValue;
            HealthSystem closestHealthSystem = null;

            foreach (Collider2D obj in hitObjects)
            {
                HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    float distance = Vector2.Distance(center, obj.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHealthSystem = healthSystem;
                    }
                }
            }

            if (closestHealthSystem != null)
            {
                closestHealthSystem.Heal(healAmount);
                Debug.Log($"{closestHealthSystem.gameObject.name} was healed by {healAmount}. Current health: {closestHealthSystem.currentHealth}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the healing zone with a green circle
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
