using UnityEngine;
using System.Collections;

public class DamageBooster : MonoBehaviour
{
    public float detectionRange = 5f; // Range for detecting ELFs
    public int BoostValue = 10; // Base damage boost amount per ELF
    public float boostDurationT = 3f; // Boost duration
    public float yOffset = 1f; // Y offset for Gizmo
    private bool pKeyPressed = false; // Tracks if P key was pressed

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !pKeyPressed)
        {
            pKeyPressed = true;
            StartCoroutine(CheckForEnemiesRoutine());
        }
    }

    private IEnumerator CheckForEnemiesRoutine()
    {
        while (pKeyPressed) // Keep checking as long as P was pressed at least once
        {
            ApplyBoostFromElfs();
            yield return new WaitForSeconds(1f); // Wait 1 second before checking again
        }
    }

    private void ApplyBoostFromElfs()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        int elfCount = 0;

        foreach (Collider2D collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Ensure it's an enemy team
            if ((gameObject.tag == "Team1" && obj.CompareTag("Team2")) || 
                (gameObject.tag == "Team2" && obj.CompareTag("Team1")))
            {
                // Count how many ELFs are detected
                if (obj.GetComponent<ELF>() != null)
                {
                    elfCount++;
                }
            }
        }

        if (elfCount > 0)
        {
            NearestEnemy myNearestEnemy = GetComponent<NearestEnemy>();

            if (myNearestEnemy != null)
            {
                int totalBoost = BoostValue * elfCount;
                myNearestEnemy.DamageBoost(totalBoost, boostDurationT);
            }
        }
    }
}
