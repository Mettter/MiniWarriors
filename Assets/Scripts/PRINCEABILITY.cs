using System.Collections;
using UnityEngine;

public class PrinceAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private int trickMana = 10;
    [SerializeField] private float battleModeDuration = 5f;
    [SerializeField] private float battleModeRange = 3f;
    [SerializeField] private float spawnCooldown = 1f;
    [SerializeField] private GameObject slashObject;
    [SerializeField] private float dashDistance = 5f; // Initial dash distance
    [SerializeField] private float dashDuration = 0.3f; // Duration of dash
    [SerializeField] private float dashCooldown = 1f; // Dash cooldown time

    private ManaSystem manaSystem;
    private bool inBattleMode = false;
    private bool canDash = true;
    private float nextAllowedDashTime = 0f;
    private bool isDashingRight = true; // Tracks direction of the dash
    private bool pKeyPressed = false; // Flag to track if the P key was pressed

    private void Start()
    {
        manaSystem = GetComponent<ManaSystem>();
    }

    private void Update()
    {
        // Track the P key press
        if (Input.GetKeyDown(KeyCode.P))
        {
            pKeyPressed = true; // Set the flag to true when P is pressed
        }

        if (manaSystem == null || manaSystem.currentMana < trickMana) return;

        if (!inBattleMode)
        {
            StartCoroutine(EnterBattleMode());
        }
    }

    private IEnumerator EnterBattleMode()
    {
        inBattleMode = true;

        // Dash only if P key was pressed at least once and there are valid targets within range
        if (canDash && pKeyPressed && Time.time >= nextAllowedDashTime && HasValidTargetsInRange())
        {
            StartCoroutine(Dash());
        }

        // Spawn slash objects at every enemy's position with offset when Battle Mode starts
        SpawnSlashObjects();

        yield return new WaitForSeconds(battleModeDuration);

        // Dash again if conditions are met
        if (canDash && pKeyPressed && Time.time >= nextAllowedDashTime && HasValidTargetsInRange())
        {
            StartCoroutine(Dash());
        }

        // Spawn slash objects at every enemy's position with offset when Battle Mode ends
        SpawnSlashObjects();

        inBattleMode = false;
    }

    private void SpawnSlashObjects()
    {
        // Find all enemies within range
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, battleModeRange);

        foreach (Collider2D col in hitObjects)
        {
            if (col.CompareTag(GetEnemyTag()) && col.GetComponent<Projectile>() == null)
            {
                Vector3 spawnPosition = col.transform.position + new Vector3(0, -0.25f, 0);
                if (pKeyPressed)
                {
                    GameObject spawnedSlash = Instantiate(slashObject, spawnPosition, Quaternion.identity);
                    spawnedSlash.tag = gameObject.tag;
                } // Set the tag of the spawned object to match the host's tag
            }
        }
    }

    private bool HasValidTargetsInRange()
    {
        // Check if there are valid targets within the battle mode range
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, battleModeRange);

        foreach (Collider2D col in hitObjects)
        {
            if (col.CompareTag(GetEnemyTag()) && col.GetComponent<Projectile>() == null)
            {
                return true; // There is at least one valid target within range
            }
        }

        return false; // No valid targets within range
    }

    private IEnumerator Dash()
    {
        canDash = false;

        // Dash direction (flip direction each time)
        Vector2 originalPosition = transform.position;
        Vector2 targetPosition;

        if (isDashingRight)
        {
            targetPosition = originalPosition + new Vector2(dashDistance, 0); // Dash to the right
        }
        else
        {
            targetPosition = originalPosition + new Vector2(-dashDistance, 0); // Dash to the left
        }

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Final position after dash

        isDashingRight = !isDashingRight; // Flip the dash direction

        nextAllowedDashTime = Time.time + dashCooldown;
        canDash = true;
    }

    private string GetEnemyTag()
    {
        return gameObject.CompareTag("Team1") ? "Team2" : "Team1";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, -0.25f, 0), battleModeRange);
    }
}