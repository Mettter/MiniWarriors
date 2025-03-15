using UnityEngine;

public class FireWizard : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballObject; // The fireball prefab to spawn
    [SerializeField] private int castMana = 10; // Mana required to cast the fireball
    [SerializeField] private float fireballOffsetY = -0.25f; // Y offset for fireball spawn

    private ManaSystem manaSystem; // Reference to the ManaSystem

    private void Start()
    {
        manaSystem = GetComponent<ManaSystem>(); // Get ManaSystem from host
        if (manaSystem == null)
        {
            Debug.LogError("ManaSystem not found on the FireWizard object!");
        }
    }

    private void Update()
    {
        if (manaSystem != null && manaSystem.currentMana >= castMana)
        {// Check if the player presses 'F' to cast the fireball
            
            CastFireball();
            
        }
    }

    private void CastFireball()
    {
        // Ensure fireballObject is set
        if (fireballObject != null)
        {
            // Calculate the position with the -0.25 Y offset
            Vector3 spawnPosition = transform.position + new Vector3(0f, fireballOffsetY, 0f);

            // Instantiate the fireball at the calculated position
            GameObject spawnedFireball = Instantiate(fireballObject, spawnPosition, Quaternion.identity);

            // Set the spawned fireball's tag to match the host object's tag
            spawnedFireball.tag = gameObject.tag;

            // Reduce mana by the amount required to cast
            manaSystem.currentMana -= castMana;
        }
        else
        {
            Debug.LogError("Fireball Object is not set!");
        }
    }
}
