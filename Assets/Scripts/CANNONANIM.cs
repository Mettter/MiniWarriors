using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    private float shootInterval; // Time between shots, now based on host's attackCooldown
    private float shootDuration = 0.3f; // Animation duration
    private bool isShootingActive = false; // Only activates after P key is pressed
    public float overlapRadius = 1.5f; // Overlap circle radius
    public Color gizmoColor = new Color(0.6f, 0f, 0.8f, 0.4f); // Purple color (RGBA)
    
    private NearestEnemy hostEnemy; // Host's NearestEnemy component
    private bool shouldDrawGizmo = false; // Controls Gizmo visibility
    private Vector2 overlapOffset = new Vector2(0f, -0.25f); // Offset for the circle

    void Start()
    {
        hostEnemy = GetComponent<NearestEnemy>(); // Get the host's NearestEnemy component
        UpdateShootInterval(); // Set initial shoot interval
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isShootingActive)
        {
            isShootingActive = true;
            shouldDrawGizmo = true; // Enable Gizmo drawing
            StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        while (isShootingActive)
        {
            UpdateShootInterval(); // Update interval before shooting
            yield return new WaitForSeconds(shootInterval);
            
            animator.SetBool("isShooting", true);
            yield return new WaitForSeconds(shootDuration);
            animator.SetBool("isShooting", false);

            CheckNearbyAllies(); // Check for allies after each shot
        }
    }

    void CheckNearbyAllies()
    {
        Vector2 checkPosition = (Vector2)transform.position + overlapOffset;
        Collider2D[] allies = Physics2D.OverlapCircleAll(checkPosition, overlapRadius);

        foreach (Collider2D ally in allies)
        {
            if (ally.gameObject == this.gameObject) continue; // Skip self

            // Check if the ally has the same tag and does NOT have a Projectile script
            if (ally.CompareTag(gameObject.tag) && ally.GetComponent<Projectile>() == null)
            {
                NearestEnemy allyEnemy = ally.GetComponent<NearestEnemy>();

                if (allyEnemy != null && allyEnemy.isRanger)
                {
                    if (hostEnemy != null)
                    {
                        // Only reduce if values are greater than 4.5
                        if (hostEnemy.minimalAttackSpeed > 4.5f)
                            hostEnemy.minimalAttackSpeed -= 0.5f;

                        if (hostEnemy.attackCooldown > 4.5f)
                        {
                            hostEnemy.attackCooldown -= 0.5f;
                            UpdateShootInterval(); // Update shoot interval after change
                        }
                    }
                }
            }
        }
    }

    void UpdateShootInterval()
    {
        if (hostEnemy != null)
            shootInterval = Mathf.Max(0.1f, hostEnemy.attackCooldown - 0.3f); // Ensure it never goes below 0.1
    }

    void OnDrawGizmos()
    {
        if (shouldDrawGizmo)
        {
            Gizmos.color = gizmoColor;
            Vector3 gizmoPosition = transform.position + (Vector3)overlapOffset;
            Gizmos.DrawWireSphere(gizmoPosition, overlapRadius);
        }
    }
}
