using System.Collections;
using System.Linq;
using UnityEngine;

public class ElfArcherAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private GameObject defenseObject; // Object to spawn
    [SerializeField] private float abilityRange = 5f;
    [SerializeField] private int abilityMana = 5; // Radius for overlap check
    [SerializeField] private Color gizmoColor = Color.green; // Color for gizmo

    private ManaSystem manaSystem; // Reference to the ManaSystem
    private Transform targetPosition; // Position of the weakest ally
    private Vector3 spawnOffset = new Vector3(0f, -0.25f, 0f); // Offset for spawning

    private void Start()
    {
        manaSystem = GetComponent<ManaSystem>(); // Get ManaSystem from host
    }

    private void Update()
    {
        if (manaSystem == null || manaSystem.currentMana <= abilityMana)
            return; // Stop if no ManaSystem or no mana

        FindWeakestAllyAndSpawn();
    }

    private void FindWeakestAllyAndSpawn()
    {
        Vector3 overlapPosition = transform.position + spawnOffset; // Apply -0.25 Y offset
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(overlapPosition, abilityRange);
        HealthSystem weakestHealthSystem = null;
        float lowestHealth = Mathf.Infinity;

        foreach (Collider2D col in hitObjects)
        {
            if (!col.CompareTag(gameObject.tag)) continue; // Check same tag
            if (col.GetComponent<Projectile>() != null) continue; // Ignore projectiles

            HealthSystem health = col.GetComponent<HealthSystem>();
            if (health != null && health.currentHealth < lowestHealth)
            {
                lowestHealth = health.currentHealth;
                weakestHealthSystem = health;
            }
        }

        if (weakestHealthSystem != null)
        {
            targetPosition = weakestHealthSystem.transform;
            SpawnDefenseObject();
        }
        manaSystem.currentMana = 0;
    }

    private void SpawnDefenseObject()
    {
        if (defenseObject != null && targetPosition != null)
        {
            // Instantiate the defense object
            GameObject spawnedObject = Instantiate(defenseObject, targetPosition.position + spawnOffset, Quaternion.identity);
            
            // Set the tag of the spawned object to be the same as the parent
            spawnedObject.tag = gameObject.tag;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 gizmoPosition = transform.position + spawnOffset; // Apply -0.25 Y offset for Gizmo
        Gizmos.DrawWireSphere(gizmoPosition, abilityRange);
    }
}
