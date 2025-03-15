using UnityEngine;

public class TpToSpawn : MonoBehaviour
{
    [SerializeField] private Transform team1SpawnPoint; // Spawn point for Team1
    [SerializeField] private Transform team2SpawnPoint; // Spawn point for Team2

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for Team1 object colliding with OtherTeamBarrier2
        if (gameObject.CompareTag("Team1") && collision.gameObject.CompareTag("Barrier"))
        {
            if (team1SpawnPoint != null)
            {
                transform.position = team1SpawnPoint.position; // Teleport to Team1 spawn
            }
        }
        // Check for Team2 object colliding with OtherTeamBarrier1
        else if (gameObject.CompareTag("Team2") && collision.gameObject.CompareTag("Barrier"))
        {
            if (team2SpawnPoint != null)
            {
                transform.position = team2SpawnPoint.position; // Teleport to Team2 spawn
            }
        }
    }

    private void Update()
    {
        // Check for the P key press to destroy barriers
    }
}
