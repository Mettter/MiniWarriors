using System.Collections;
using UnityEngine;

public class RangeBoostTile : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float rangeBoostAmount = 2f; // Range boost amount
    [SerializeField] private float rangeBoostDuration = 1f; // Duration of the range boost
    [SerializeField] private float radius = 3f; // Radius of the boost zone
    [SerializeField] private float yOffset = 1f; // Y offset for the circle's position

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = new Color(0.5f, 0f, 0.5f); // Purple color (RGB: 128, 0, 128)

    private bool hasPressedP = false; // Tracks whether the player has pressed the "P" key
    private bool isCooldown = false; // Tracks if the boost is on cooldown

    private void Update()
    {
        // Check if "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            hasPressedP = true; // Set to true when the player presses "P"
            Debug.Log("P key pressed. Range Boost will now be applied.");
        }
    }

    private void FixedUpdate()
    {
        // Only check for targets if the P key has been pressed and not on cooldown
        if (hasPressedP && !isCooldown)
        {
            ApplyRangeBoost();
            StartCoroutine(StartCooldown(1f)); // 1-second cooldown
        }
    }

    private void ApplyRangeBoost()
    {
        // Detect objects within the boost zone
        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D obj in hitObjects)
        {
            NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
            if (enemy != null) // Ensure the object has the NearestEnemy component
            {
                enemy.RangeBoost(rangeBoostAmount, rangeBoostDuration); // Apply range boost
                Debug.Log($"{obj.name} received a range boost of {rangeBoostAmount} for {rangeBoostDuration} seconds.");
            }
        }
    }

    private IEnumerator StartCooldown(float cooldownTime)
    {
        isCooldown = true; // Set cooldown active
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false; // Reset cooldown
    }

    private void OnDrawGizmos()
    {
        // Draw the circle to visualize the boost zone with purple color
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
