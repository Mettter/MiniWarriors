using UnityEngine;

public class ArchWizard : MonoBehaviour
{
    public GameObject wallObject; // Prefab of the wall to spawn
    public Transform spawnPosition; // Where the wall will be spawned
    public int castMana = 10; // Mana cost to cast the wall
    
    private ManaSystem manaSystem;

    void Start()
    {
        // Get the ManaSystem component from the host (parent or attached object)
        manaSystem = GetComponent<ManaSystem>();
        
        if (manaSystem == null)
        {
            Debug.LogError("ManaSystem component not found on host!");
        }
    }

    void Update()
    {
        CastWall();
    }

    public void CastWall()
    {
        if (manaSystem != null && manaSystem.currentMana >= castMana)
        {
            // Set mana to 0
            manaSystem.currentMana = 0;
            
            // Spawn the wall object at the specified position
            GameObject spawnedWall = Instantiate(wallObject, spawnPosition.position, Quaternion.identity);
            
            // Set the spawned object's tag to match the parent's tag
            spawnedWall.tag = gameObject.tag;
        }
        else
        {
            Debug.Log("Not enough mana to cast wall!");
        }
    }
}