using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static int Team1Count = 0;  // Track the number of living Team1 units
    public static int Team2Count = 0;  // Track the number of living Team2 units

    // Optional: You can use this method to manually check the counts (e.g., for debugging)
    public static void PrintCounts()
    {
        Debug.Log("Team1 Count: " + Team1Count);
        Debug.Log("Team2 Count: " + Team2Count);
    }

    // Optional: You can reset the counts if needed (e.g., on level restart)
    public static void ResetCounts()
    {
        Team1Count = 0;
        Team2Count = 0;
    }
}
