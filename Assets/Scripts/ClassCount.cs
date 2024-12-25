using UnityEngine;

public class ClassesCount : MonoBehaviour
{
    [Header("Team Manager Reference")]
    [SerializeField] private GameObject teamManager;

    private ClassManager classManager;
    private HealthSystem healthSystem;

    private int lastHumanCount = 0; // Keeps track of the last checked human count

    private void Start()
    {
        if (teamManager != null)
        {
            classManager = teamManager.GetComponent<ClassManager>();
            if (classManager == null)
            {
                Debug.LogError("ClassManager component not found on TeamManager!");
            }
        }
        else
        {
            Debug.LogError("TeamManager object is not assigned in ClassesCount!");
        }

        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem component not found on this object!");
        }
    }

    private void Update()
    {
        // Constantly check and update health if human count changes
        if (classManager != null)
        {
            int currentHumanCount = classManager.humanCount;

            if (currentHumanCount != lastHumanCount)
            {
                AdjustHealthBasedOnHumans(currentHumanCount);
                lastHumanCount = currentHumanCount;
            }
        }
    }

    private void AdjustHealthBasedOnHumans(int humanCount)
    {
        if (healthSystem == null) return;

        // Check the current human count and apply the bonus health
        int bonusHealth = 0;

        if (humanCount >= 2)
        {
            bonusHealth = 10 + (humanCount - 2) * 5; // 10 for the first 2 humans, +5 for each additional
            bonusHealth = Mathf.Clamp(bonusHealth, 10, 40); // Cap the bonus at 40 for a max of 8 humans
        }

        // Apply the health changes directly to maxHealth and currentHealth
        healthSystem.maxHealth = healthSystem.maxHealth + bonusHealth;
        healthSystem.currentHealth = Mathf.Min(healthSystem.currentHealth, healthSystem.maxHealth); // Ensure currentHealth does not exceed maxHealth

        // Log health adjustment for debugging
        Debug.Log($"Health updated for {humanCount} humans: MaxHealth = {healthSystem.maxHealth}, CurrentHealth = {healthSystem.currentHealth}");
    }
}
