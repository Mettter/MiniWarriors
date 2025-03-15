using UnityEngine;

public class ForceSkeletisation : MonoBehaviour
{
    public float detectionRadius = 2f; // Radius of the overlap circle
    public float yOffset = 0f; // Vertical offset for the detection area
    public GameObject passObject; // Object to pass as skeletonObject

    private string myTeam; // Team of this object
    private string enemyTeam; // Enemy team

    void Start()
    {
        myTeam = gameObject.tag;
        enemyTeam = (myTeam == "Team1") ? "Team2" : "Team1"; // Determine the opposite team
        Debug.Log($"ForceSkeletisation on {gameObject.name} | My Team: {myTeam} | Enemy Team: {enemyTeam}");
    }

    void Update()
    {
        // Adjusted position for OverlapCircle based on yOffset
        Vector2 detectionPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPosition, detectionRadius);

        foreach (Collider2D hit in hits)
        {
            GameObject target = hit.gameObject;

            // Skip self and projectiles
            if (target == gameObject || target.GetComponent<Projectile>() != null) continue;

            // Only apply SKELETISATION if the target is on the opposite team
            if (target.CompareTag(enemyTeam))
            {
                ApplySkeletisation(target);
            }
        }
    }

    private void ApplySkeletisation(GameObject target)
    {
        if (target.GetComponent<SKELETISATION>() == null)
        {
            Debug.Log($"Applying SKELETISATION to {target.name}");
            SKELETISATION skeletisation = target.AddComponent<SKELETISATION>();
            skeletisation.skeletonPrefab = passObject; // Pass skeleton object
            skeletisation.skeletonTeam = myTeam; // Set skeleton's team to match the host's team
            Debug.Log($"SKELETISATION applied to {target.name} | Skeleton Team: {myTeam}");
        }
    }

    // Draw the detection radius in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 gizmoPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
        Gizmos.DrawWireSphere(gizmoPosition, detectionRadius);
    }
}
