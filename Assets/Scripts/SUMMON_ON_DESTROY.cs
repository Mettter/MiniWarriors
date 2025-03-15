using UnityEngine;

public class DestroyAndSpawnOnCollision : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private bool destroySelfAfterCollision = false; // Destroy functionality is enabled only if this is true
    [SerializeField] private bool noCondition = false; // Destroy itself unconditionally
    [SerializeField] private float deathDelay = 0f; // Delay before destruction
    [SerializeField] private bool destroyOnlySelf = false;
    
    [SerializeField] private bool destroyafterP = false; // Destroy only this object on collision

    [SerializeField] private float pKeyPressedTime = 0;
    [SerializeField] private float rangeV = 0.1f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn; // The prefab to spawn
    [SerializeField] private float spawnYOffset = 0f; // Vertical offset for the spawn position
    [SerializeField] private bool spawnInCollision = false;
    [SerializeField] private bool inhabitTag = true; // Spawn in the center of the object collided with
    [SerializeField] private bool spawnNoMatterWhat = false; // Always spawn the prefab, regardless of other conditions
    [SerializeField] private bool spawnOnDestroy = false; // Spawn prefab when the host object is destroyed

    // Track if the P key was pressed at least once
    private static bool isPKeyPressed = false;

    private void Start()
    {
        // If noCondition is true, schedule self-destruction, but only if destroySelfAfterCollision is true
        if (noCondition && destroySelfAfterCollision)
        {
            Destroy(gameObject, deathDelay);
        }
        pKeyPressedTime = 0f;
    }

    private void Update()
    {
        // Check if the P key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPKeyPressed = true;
            Debug.Log("P key was pressed. Collision and trigger now active.");
            pKeyPressedTime = Time.time;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject, collision.contacts[0].point);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject, other.bounds.center);
    }

    private void HandleCollision(GameObject collidedObject, Vector3 collisionPoint)
    {
        // Only spawn prefab if one of the following is true:
        // - spawnNoMatterWhat is true
        // - destroySelfAfterCollision is true and other specific conditions are met
        if (spawnNoMatterWhat || (destroySelfAfterCollision && isPKeyPressed))
        {
            Vector3 spawnPosition = spawnInCollision
                ? collisionPoint + new Vector3(0, spawnYOffset, 0) // Spawn at collision point with Y offset
                : transform.position + new Vector3(0, spawnYOffset, 0); // Spawn at current object position with Y offset

            SpawnPrefab(spawnPosition);
        }

        // Only process destruction logic if destroySelfAfterCollision is enabled
        if (!destroySelfAfterCollision) return;

        // If destroyOnlySelf is true, destroy only this object
        if (destroyOnlySelf)
        {
            Destroy(gameObject, deathDelay);
            return;
        }

        // Only activate destruction after the P key was pressed
        if (!isPKeyPressed) return;

        // Destroy the collided object after delay
        
        Destroy(collidedObject, deathDelay);
        Debug.Log($"Scheduled destruction for {collidedObject.name}");
         Destroy(gameObject, deathDelay);
        
    }

    private void OnDestroy()
    {
        // Prevent spawning if the application is exiting or the prefab to spawn is not assigned
        if (!spawnOnDestroy || prefabToSpawn == null || !Application.isPlaying) return;

        // Calculate the spawn position with the vertical offset
        Vector3 spawnPosition = transform.position + new Vector3(0, spawnYOffset, 0);

        // Instantiate the prefab
        SpawnPrefab(spawnPosition);
    }

    private void SpawnPrefab(Vector3 position)
{
    if (prefabToSpawn != null)
    {

        // Check if this object is colliding with a "Player" tagged object
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rangeV);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                Debug.Log("Cannot spawn prefab: Colliding with Player.");
                return; // Prevent spawning
            }
        }

        GameObject spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
        spawnedObject.tag = gameObject.tag; // Assign the same tag to the spawned object

        // Ensure the collider is not set as trigger
        Collider2D collider = spawnedObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false; // Set to false or true based on your requirements
        }

        Debug.Log($"Spawned {prefabToSpawn.name} at {position}");
    }
    else
    {
        Debug.LogWarning("Prefab to spawn is not assigned!");
    }
}


}
