using UnityEngine;

public class ManaBasedBlockDistributor : MonoBehaviour
{
    public ManaSystem manaSystem;       // Reference to the ManaSystem
    public float ultimateMana = 100f;  // The amount of mana required to trigger block distribution
    private float range = 100f;        // The range of the invisible overlap circle

    private void Update()
    {
        // Check if currentMana is equal to or greater than ultimateMana
        if (manaSystem != null && manaSystem.currentMana >= ultimateMana)
        {
            // Reset mana and distribute block
            manaSystem.currentMana = 0;
            DistributeBlock();
        }
    }

    private void DistributeBlock()
    {
        // Find all objects within the overlap circle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            // Check if the object's tag matches the host object's tag
            if (hit.CompareTag(gameObject.tag))
            {
                // Get the HealthSystem component on the object
                HealthSystem healthSystem = hit.GetComponent<HealthSystem>();

                if (healthSystem != null)
                {
                    // Add block to the object
                    healthSystem.AddBlock(5);
                    Debug.Log($"Added block to: {hit.name}");
                }
            }
        }
    }

    // Optional: Visualize the range in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
