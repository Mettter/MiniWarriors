using System.Collections;
using UnityEngine;

public class CollisionDashTrigger : MonoBehaviour
{
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;
    private bool hasDashed = false; // Ensures only one forced dash happens

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryForceDash(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryForceDash(other.gameObject);
    }

    private void TryForceDash(GameObject obj)
    {
        if (hasDashed || obj.GetComponent("Projectile") || obj.GetComponent("NOMOVE")) return; // Stops after one dash and ignores projectiles

        string selfTag = gameObject.tag;
        string targetTag = obj.tag;

        // Only force dash on an opposite team object
        if ((selfTag == "Team1" && targetTag == "Team2") || (selfTag == "Team2" && targetTag == "Team1"))
        {
            hasDashed = true; // Prevent further forced dashes
            StartCoroutine(DashObject(obj));
        }
    }

    private IEnumerator DashObject(GameObject obj)
    {
        Vector2 originalPosition = obj.transform.position;
        Vector2 targetPosition;

        // Check the tag of the collided object to determine the direction
        if (obj.CompareTag("Team2")) // If the collided object has Team2 tag, dash to the right
        {
            targetPosition = originalPosition + new Vector2(dashDistance, 0);
        }
        else if (obj.CompareTag("Team1")) // If the collided object has Team1 tag, dash to the left
        {
            targetPosition = originalPosition + new Vector2(-dashDistance, 0);
        }
        else
        {
            yield break; // If it's neither Team1 nor Team2, exit the coroutine
        }

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            obj.transform.position = Vector2.Lerp(originalPosition, targetPosition, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition;
    }
}
