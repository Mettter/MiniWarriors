using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public float knockBackPower = 20f; // Force applied during knockback
    public float knockBackDistance = 5f; // Desired distance in tiles
    public ForceMode forceMode = ForceMode.VelocityChange; // Impulse for immediate push

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody collidingRigidbody = collision.rigidbody;

        if (collidingRigidbody != null)
        {
            // Calculate direction of knockback
            Vector3 knockBackDirection = (collision.transform.position - transform.position).normalized;

            // Scale force based on desired knockback distance
            float force = Mathf.Max(knockBackPower, knockBackDistance / Time.fixedDeltaTime);

            // Apply force
            collidingRigidbody.AddForce(knockBackDirection * force * 100);

            Debug.Log($"{collision.gameObject.name} knocked back with a force of {force}!");
        }
    }
}
