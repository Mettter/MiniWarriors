using System.Collections;
using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // Prefab to instantiate as a projectile
    [SerializeField] private float attackCooldown = 1f; // Cooldown time between attacks
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public float ultimateMana = 100f;   // Mana cost for ultimate ability

    private bool isCurrentlyAttacking = false; // Tracks if the object is currently attacking

    private void Start()
    {
        if (manaSystem == null)
        {
            manaSystem = GetComponent<ManaSystem>();
            if (manaSystem == null)
            {
                Debug.LogWarning("ManaSystem component is missing on the GameObject!");
            }
        }
    }

    private void Update()
    {
        // Only spawn the projectile if ultimate mana is available and we're not currently attacking
        if (manaSystem != null && manaSystem.currentMana >= ultimateMana && !isCurrentlyAttacking)
        {
            StartCoroutine(SpawnProjectile());
        }
    }

    private IEnumerator SpawnProjectile()
    {
        if (isCurrentlyAttacking)
            yield break;

        // Deduct ultimate mana
        manaSystem.currentMana -= ultimateMana;
        isCurrentlyAttacking = true;

        // Calculate the spawn position with both x and y offsets right before spawning
        Vector3 spawnPosition = transform.position; // Use the live position of the host object

        // Wait for attack cooldown
        yield return new WaitForSeconds(attackCooldown);

        // Spawn the projectile at the live position of the host object
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Wait for attack cooldown
        yield return new WaitForSeconds(attackCooldown);

        isCurrentlyAttacking = false;
    }
}
