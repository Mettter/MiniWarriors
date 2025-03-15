using UnityEngine;
using System.Collections.Generic;

public class DamageGiver : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius to search for nearby objects
    public int BoostValue = 10; // Boost value for damage increase
    public float damageBoostDuration = 3f; // Duration of the boost
    public float yOffset = 1f; // Y offset for gizmo drawing
    public GameObject effectPrefab;
    private bool chatgprbesmarter = false; // Prefab for the effect that appears when boosting

    private HashSet<GameObject> boostedObjects = new HashSet<GameObject>(); // Track boosted objects

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Only apply boost when P is pressed
        {
            chatgprbesmarter = true;
        }
        if(chatgprbesmarter)
        {
            ApplyDamageBoost();
        }
    }

    private void ApplyDamageBoost()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Skip if the object already received the boost
            if (boostedObjects.Contains(obj))
                continue;

            // Skip objects with a different tag or with the Projectile script
            if (obj.CompareTag(gameObject.tag) && obj.GetComponent<Projectile>() == null)
            {
                NearestEnemy enemyScript = obj.GetComponent<NearestEnemy>();

                if (enemyScript != null)
                {
                    // Apply damage boost and mark as boosted
                    enemyScript.DamageBoost(BoostValue, damageBoostDuration);
                    boostedObjects.Add(obj);

                    // Spawn effect at gizmo position
                    Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
                    Instantiate(effectPrefab, effectPosition, Quaternion.identity);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Vector3 gizmoPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        Gizmos.DrawWireSphere(gizmoPosition, detectionRadius);
    }
}
