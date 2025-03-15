using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool destroySelfAfterCollision = false; // Destroy itself after a collision
    [SerializeField] private bool noCondition = false; // Destroy itself unconditionally
    [SerializeField] private float deathDelay = 0f; // Delay before destruction
    [SerializeField] private bool destroyOnlySelf = false;
    [SerializeField] private bool noConditionButPkey = false;// Destroy only this object on collision

    // Track if the P key was pressed at least once
    private static bool isPKeyPressed = false;

    private void Start()
    {
        // If noCondition is true, schedule self-destruction
        if (noCondition)
        {
            Destroy(gameObject, deathDelay);
        }
    }

    private void Update()
    {
        // Check if the P key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPKeyPressed = true;
            Debug.Log("P key was pressed. Collision and trigger now active.");
            if (noConditionButPkey)
            {
                Destroy(gameObject, deathDelay);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If destroyOnlySelf is true, destroy only this object
        if (destroyOnlySelf)
        {
            Destroy(gameObject, deathDelay);
            return;
        }

        // Only activate after the P key was pressed
        if (!isPKeyPressed) return;

        // Destroy the collided object after delay
        Destroy(collision.gameObject, deathDelay);
        Debug.Log($"Scheduled destruction for {collision.gameObject.name}");

        // Optionally destroy the object with this script
        if (destroySelfAfterCollision)
        {
            Destroy(gameObject, deathDelay);
        }
    }

}
