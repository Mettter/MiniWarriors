using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    [SerializeField] private float maxMana = 100f;   // Maximum mana the player can have
    [SerializeField] private float starterMana = 50f; // Starting mana when the game begins
    public float currentMana;                        // The current amount of mana the player has

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
    }

    // Method to reduce mana from currentMana
    public void ReduceMana(float amount)
    {
        currentMana -= amount;

        // Ensure currentMana doesn't go below 0
        if (currentMana < 0)
        {
            currentMana = 0;
        }
    }
}
