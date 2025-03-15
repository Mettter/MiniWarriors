using UnityEngine;

public class NOMOVE : MonoBehaviour
{
    public GameObject backlashObject;  // Reference to the backlash object prefab
    private Vector3 savedPosition;     // Variable to store the position when P key is pressed
    private bool isPositionSaved = false;  // Flag to check if position has been saved

    public float spawnCooldown = 0.5f;  // Cooldown time in seconds before spawning backlashObject
    private float spawnCooldownTimer = 0f;
    public bool isPrefab = false;  // Timer to track cooldown

    void Update()
    {
        // Decrease the cooldown timer over time
        if (spawnCooldownTimer > 0f)
        {
            spawnCooldownTimer -= Time.deltaTime;
        }

        // Check if P key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Save the object's position
            savedPosition = transform.position;
            isPositionSaved = true;
        }

        if(isPrefab)
        {
            savedPosition = transform.position;
            isPositionSaved = true;
        }

        // If position is saved, check if the object is not in the saved position
        if (isPositionSaved)
        {
            // If the object is not in the saved position and cooldown has passed, spawn backlashObject
            if (transform.position != savedPosition && spawnCooldownTimer <= 0f)
            {
                SpawnBacklashObject();
                spawnCooldownTimer = spawnCooldown;  // Reset the cooldown timer
            }

            // Keep teleporting the object back to the saved position
            transform.position = savedPosition;
        }
    }

    // Function to spawn the backlash object at the saved position with a Y offset
    private void SpawnBacklashObject()
    {
        if (backlashObject != null)
        {
            // Instantiate the backlash object at the saved position with a -0.25 Y offset and no rotation
            Vector3 spawnPosition = new Vector3(savedPosition.x, savedPosition.y - 0.25f, savedPosition.z);
            GameObject spawnedObject = Instantiate(backlashObject, spawnPosition, Quaternion.identity);

            // Assign the same tag as the current object
            spawnedObject.tag = gameObject.tag;
        }
    }
}
