using UnityEngine;
using System.Collections;

public class TORNADOSPELL : MonoBehaviour
{
    public float tornadoPullSpeed = 5f;
    public float pullRange = 5f;
    public float stopDistance = 0.1f; // Distance at which pulling stops
    public float yOffset = 1f; // Y offset for pulling position
    private string enemyTag;
    private bool isActivated = false; // Track if the tornado is active
    private bool isActivationPending = false; // Track if activation is pending
    public float activationDelay = 1f; // Delay before activation

    void Start()
    {
        // Determine enemy tag based on tornado's tag
        enemyTag = gameObject.tag == "Team1" ? "Team2" : "Team1";
    }

    void Update()
    {
        // Start the activation delay when "P" is pressed
        if (Input.GetKeyDown(KeyCode.P) && !isActivationPending && !isActivated)
        {
            isActivationPending = true;
            StartCoroutine(ActivateTornadoAfterDelay());
        }

        if (!isActivated) return; // Do nothing if not activated

        // Find all colliders within circular range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRange);

        foreach (Collider2D col in colliders)
        {
            // Skip objects that have the Projectile script
            if (col.GetComponent<Projectile>() != null)
                continue;

            if (col.CompareTag(enemyTag))
            {
                Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
                Vector2 pullDirection = (targetPosition - (Vector2)col.transform.position);

                // Stop pulling if the object is already at the target position
                if (pullDirection.magnitude > stopDistance)
                {
                    col.transform.position += (Vector3)pullDirection.normalized * tornadoPullSpeed * Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator ActivateTornadoAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        isActivated = true;
        isActivationPending = false;
    }

    void OnDrawGizmos()
    {
        // Visualize the pull range with a circle
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRange);
    }
}
