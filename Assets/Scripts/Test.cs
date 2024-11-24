using UnityEngine;

public class AttackSpeedBoost : MonoBehaviour
{
    // Reference to the NearestEnemy script
    private NearestEnemy nearestEnemy;

    private float boostAmount = 999f;
    private float boostDuration = 10f;

    void Start()
    {
        // Get the NearestEnemy component from the same GameObject this script is attached to
        nearestEnemy = GetComponent<NearestEnemy>();
    }

    void Update()
    {
        // Check if the 'K' key is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Apply the attack speed boost if nearestEnemy is assigned
            if (nearestEnemy != null)
            {
                nearestEnemy.AttackSpeedBoost(boostAmount, boostDuration);
            }
            else
            {
                Debug.LogError("NearestEnemy component not found on this GameObject.");
            }
        }
    }
}
