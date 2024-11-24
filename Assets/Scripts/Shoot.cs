using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // The projectile prefab to spawn
    [SerializeField] private Transform target; // Target for the projectile
    [SerializeField] private float shootSpeed = 1f; // Time between shots
    [SerializeField] private float initialProjectileSpeed = 5f; // Initial speed of the projectile
    [SerializeField] private float speedDecay = 0.5f; // Speed loss per decay interval
    [SerializeField] private float decayInterval = 0.5f; // Time between speed decays

    private float shootTimer; // Timer for shooting intervals

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            shootTimer = shootSpeed;
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = gameObject.tag; // Match the projectile's tag with the shooter
        StartCoroutine(MoveProjectile(projectile));
    }

    private IEnumerator MoveProjectile(GameObject projectile)
    {
        float currentSpeed = initialProjectileSpeed;
        Transform projectileTransform = projectile.transform;

        // Ensure we can find the sprite renderer for flipping
        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Projectile prefab is missing a SpriteRenderer component.");
        }

        while (projectile != null && target != null)
        {
            Vector3 direction = (target.position - projectileTransform.position).normalized;

            // Flip the sprite based on the movement direction
            FlipProjectileSprite(spriteRenderer, direction);

            // Move the projectile
            projectileTransform.position += direction * currentSpeed * Time.deltaTime;

            // Wait for decay interval
            yield return new WaitForSeconds(decayInterval);

            // Apply speed decay
            currentSpeed = Mathf.Max(0, currentSpeed - speedDecay);

            // Stop moving if the speed reaches 0 or close to the target
            if (currentSpeed <= 0f || Vector3.Distance(projectileTransform.position, target.position) < 0.1f)
            {
                Destroy(projectile);
                break;
            }
        }
    }

    private void FlipProjectileSprite(SpriteRenderer spriteRenderer, Vector3 direction)
    {
        if (spriteRenderer != null)
        {
            // Flip horizontally based on the direction's x-component
            spriteRenderer.flipX = direction.x < 0;
        }
    }
}
