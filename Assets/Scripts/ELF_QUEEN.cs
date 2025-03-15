using UnityEngine;
using System.Collections;

public class ManaDistributor : MonoBehaviour
{
    [SerializeField] private float range = 5f;       // The range of the overlap circle
    [SerializeField] private float manaAmount = 10f; // Amount of mana to add to each object
    [SerializeField] private GameObject effectObject; // Effect prefab to spawn
    [SerializeField] private float effectYOffset = -0.25f; // Y offset for effect
    [SerializeField] private Animator animator;   // Animator reference
    private Coroutine manaDistributionCoroutine;    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (manaDistributionCoroutine != null)
            {
                StopCoroutine(manaDistributionCoroutine);
            }
            manaDistributionCoroutine = StartCoroutine(DelayedDistributeMana());
        }
    }

    private IEnumerator DelayedDistributeMana()
    {
        if (animator != null)
        {
            animator.SetBool("isCasting", true);
        }
        
        yield return new WaitForSeconds(0.1f);

        if (this != null && gameObject != null)
        {
            DistributeMana();
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (animator != null)
        {
            animator.SetBool("isCasting", false);
        }
    }

    private void DistributeMana()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(gameObject.tag))
            {
                ManaSystem manaSystem = hit.GetComponent<ManaSystem>();

                if (manaSystem != null)
                {
                    manaSystem.AddMana(manaAmount);
                    manaSystem.manaPerSecond += 1;

                    Debug.Log($"{hit.gameObject.name} received {manaAmount} mana. manaPerSecond increased to {manaSystem.manaPerSecond}.");
                }
            }
        }

        SpawnEffect();
    }

    private void SpawnEffect()
    {
        if (effectObject != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + effectYOffset, transform.position.z);
            Instantiate(effectObject, spawnPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
