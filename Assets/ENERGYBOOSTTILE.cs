using UnityEngine;
using System.Collections;

public class ManaAdder : MonoBehaviour
{
    [SerializeField] private float range = 5f;       // The range of the overlap circle
    [SerializeField] private float manaAmount = 10f; // Amount of mana to add to each object
    private Coroutine manaDistributionCoroutine;     // Coroutine reference to handle delay     // Animator for casting visuals

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

    // Coroutine to handle delay
    private IEnumerator DelayedDistributeMana()
    {

        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Distribute mana if the object is still active
        if (this != null && gameObject != null)
        {
            DistributeMana();
        }

        yield return new WaitForSeconds(0.2f);
    }

    // Method to distribute mana to all eligible objects
    private void DistributeMana()
    {
        // Find all colliders within the specified range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            // Try to get the ManaSystem component
            ManaSystem manaSystem = hit.GetComponent<ManaSystem>();

            if (manaSystem != null)
            {
                // Add mana to the ManaSystem
                manaSystem.AddMana(manaAmount);
                Debug.Log($"{hit.gameObject.name} received {manaAmount} mana.");
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
