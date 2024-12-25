using UnityEngine;
using System.Collections;

public class ManaDistributor : MonoBehaviour
{
    [SerializeField] private float range = 5f;       // The range of the overlap circle
    [SerializeField] private float manaAmount = 10f; // Amount of mana to add to each object
    private Coroutine manaDistributionCoroutine;    
    [SerializeField] private Animator animator;   // Coroutine reference to handle delay and cancellation

    private void Update()
    {
        // When the P key is pressed, start the delayed mana distribution
        if (Input.GetKeyDown(KeyCode.P))
        {
            // If a coroutine is already running, stop it to prevent overlapping calls
            if (manaDistributionCoroutine != null)
            {
                StopCoroutine(manaDistributionCoroutine);
            }
            
            manaDistributionCoroutine = StartCoroutine(DelayedDistributeMana());
        }
    }

    // Coroutine to handle delay and cancellation
    private IEnumerator DelayedDistributeMana()
    {
        // Wait for 0.1 seconds
        if (animator != null)
        {
            animator.SetBool("isCasting", true);
        }
        
        yield return new WaitForSeconds(0.1f);

        // Check if the host object is still active
        if (this != null && gameObject != null)
        {
            DistributeMana(); // Call the mana distribution method
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (animator != null)
        {
            animator.SetBool("isCasting", false);
        }
        
    }

    // Method to distribute mana and increase manaPerSecond for all eligible objects
    private void DistributeMana()
    {
        // Find all colliders within the specified range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        // Iterate through each hit collider
        foreach (Collider2D hit in hits)
        {
            // Check if the object's tag matches this object's tag
            if (hit.CompareTag(gameObject.tag))
            {
                // Try to get the ManaSystem component
                ManaSystem manaSystem = hit.GetComponent<ManaSystem>();

                if (manaSystem != null)
                {
                    // Add mana to the ManaSystem
                    manaSystem.AddMana(manaAmount);

                    // Increment manaPerSecond by 1
                    manaSystem.manaPerSecond += 1;

                    Debug.Log($"{hit.gameObject.name} received {manaAmount} mana. manaPerSecond increased to {manaSystem.manaPerSecond}.");
                }
            }
        }
    }

    // Optional: Visualize the overlap circle in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
