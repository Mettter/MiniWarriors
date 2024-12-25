using UnityEngine;
using System.Collections;

public class ManaSystem : MonoBehaviour
{
    [SerializeField] private float maxMana = 100f;       // Maximum mana the player can have
    [SerializeField] private float starterMana = 50f;     // Starting mana when the game begins
    public float currentMana;                             // The current amount of mana the player has
    [SerializeField] public float manaPerSecond = 0f;    // Mana added per second when "P" is pressed

    private bool hasPressedP = false;                     // Flag to check if "P" was pressed at least once

    // Property to get the current mana
    public float CurrentMana => currentMana;
    // Property to get the max mana
    public float MaxMana => maxMana;

    private void Start()
    {
        // Set current mana to starterMana at the start
        currentMana = starterMana;

        // If currentMana exceeds maxMana, clamp it to maxMana
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        Debug.Log($"Mana initialized. Starting mana: {currentMana}");
    }

    private void Update()
    {
        // Check if the P key has been pressed at least once
        if (Input.GetKeyDown(KeyCode.P))
        {
            hasPressedP = true;
            StartCoroutine(RegenerateManaOverTime()); // Start regenerating mana when P is pressed
        }
    }

    // Coroutine to regenerate mana over time if P is pressed
    private IEnumerator RegenerateManaOverTime()
    {
        while (hasPressedP)
        {
            // Add manaPerSecond to currentMana
            AddMana(manaPerSecond);

            // Wait for 1 second before adding more mana
            yield return new WaitForSeconds(1f);
        }
    }

    // Method to add mana to currentMana
    public void AddMana(float amount)
    {
        currentMana += amount;

        // Ensure currentMana doesn't exceed maxMana
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        Debug.Log($"Mana added. New mana: {currentMana}");
    }

    // Method to reduce mana from currentMana
    public void ReduceMana(float amount)
    {
        Debug.Log($"Attempting to reduce mana by {amount}. Current mana: {currentMana}");

        currentMana -= amount;

        // Ensure currentMana doesn't go below 0
        if (currentMana < 0)
        {
            currentMana = 0;
        }

        Debug.Log($"Mana reduced by {amount}. Current mana: {currentMana}");
    }
}
