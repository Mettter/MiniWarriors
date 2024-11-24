using UnityEngine;

public class imAlive : MonoBehaviour
{
    public Animator animator;  // Reference to the Animator component

    // Static variables for team counts, still referenced in TeamManager
    [SerializeField] public static int Team1Count = 0;  
    [SerializeField] public static int Team2Count = 0; 

    // Public variables to display team counts in the Inspector
    public int showTeamCount1;  // Will display Team1 count in the Inspector
    public int showTeamCount2;  // Will display Team2 count in the Inspector

    private void Update()
    {
        // Continuously update the showTeamCount variables to reflect TeamManager's static counts
        showTeamCount1 = TeamManager.Team1Count;
        showTeamCount2 = TeamManager.Team2Count;

        // Optional: You can log the counts for debugging purposes
        Debug.Log("Team 1 Count: " + showTeamCount1);
        Debug.Log("Team 2 Count: " + showTeamCount2);
    }

    private void OnEnable()
    {
        // Check which team the unit belongs to and update the count in the global manager
        if (gameObject.CompareTag("Team1"))
        {
            TeamManager.Team1Count++;
        }
        else if (gameObject.CompareTag("Team2"))
        {
            TeamManager.Team2Count++;
        }

        // Check counts and update animator
        UpdateAnimator();
    }

    private void OnDestroy()
    {
        // When the unit dies or is destroyed, decrease the count for the respective team
        if (gameObject.CompareTag("Team1"))
        {
            TeamManager.Team1Count--;
        }
        else if (gameObject.CompareTag("Team2"))
        {
            TeamManager.Team2Count--;
        }

        // Check counts and update animator
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        // Ensure animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component is missing on the GameObject!");
                return;
            }
        }

        // Set isWalking to false if either team count is 0
        if (TeamManager.Team1Count == 0 || TeamManager.Team2Count == 0)
        {
            animator.SetBool("isWalking", false);
            Debug.Log("Set isWalking to false because one team count is 0.");
        }
    }
}
