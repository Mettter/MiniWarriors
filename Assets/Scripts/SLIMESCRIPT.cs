using UnityEngine;

public class ManaAction : MonoBehaviour
{
    public GameObject spawnObject; // The object to spawn
    private ManaSystem manaSystem; // Reference to the ManaSystem script
    public int actionMana; // The actionMana value to compare with currentMana

    void Start()
    {
        // Try to get the ManaSystem component from the host object (this object)
        manaSystem = GetComponent<ManaSystem>();
        
        if (manaSystem == null)
        {
            Debug.LogError("ManaSystem script not found on the host object!");
        }
    }

    void Update()
    {
        // Check if actionMana equals the currentMana value from ManaSystem
        if (manaSystem != null && actionMana == manaSystem.currentMana)
        {
            // Spawn the object at the host's position with -0.25 Y offset
            Vector3 spawnPosition = transform.position;
            spawnPosition.y -= 0f; // Adjusting the Y position

            // Instantiate the spawnObject
            GameObject spawnedObj = Instantiate(spawnObject, spawnPosition, Quaternion.identity);

            // Set the tag of the spawned object to be the same as the host's tag
            spawnedObj.tag = gameObject.tag;

            // Destroy the host object
            Destroy(gameObject);
        }
    }
}
