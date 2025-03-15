using System.Collections;
using UnityEngine;

public class GrandmaBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float rangeBoostAmount = 2f;
    [SerializeField] private float attackSpeedBoostAmount = 0.5f;
    [SerializeField] private float attackSpeedBoostDuration = 1f;
    [SerializeField] private float rangeBoostDuration = 1f;
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float armorBoostValue = 5f;
    [SerializeField] private bool onlyRangers = true;

    [Header("Visual Settings")]
    [SerializeField] private Color boostZoneColor = new Color(0.5f, 0f, 0.5f);

    private bool hasPressedP = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !hasPressedP)
        {
            hasPressedP = true;
            Debug.Log("P key pressed. Boost will be applied after 1 second.");
            StartCoroutine(ApplyBoostOnce()); // Apply boost only once after a delay
        }
    }

    private IEnumerator ApplyBoostOnce()
    {
        yield return new WaitForSeconds(1f); // Delay before applying boost

        Vector2 center = new Vector2(transform.position.x, transform.position.y + yOffset);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag(gameObject.tag))
            {
                NearestEnemy enemy = obj.GetComponent<NearestEnemy>();
                if (enemy != null)
                {
                    if (onlyRangers && enemy.isRanger || !onlyRangers)
                    {
                        enemy.RangeBoost(rangeBoostAmount, rangeBoostDuration);
                        enemy.AttackSpeedBoost(attackSpeedBoostAmount, attackSpeedBoostDuration);
                    }
                }

                HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
                if (healthSystem != null && !IsArmorBoostActive(healthSystem))
                {
                    healthSystem.armorPoints += armorBoostValue;
                    Debug.Log($"{obj.name} received {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
                    StartCoroutine(RemoveArmorBoost(healthSystem, armorBoostValue, boostDuration));
                }
            }
        }
    }

    private IEnumerator RemoveArmorBoost(HealthSystem healthSystem, float armorBoostValue, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (healthSystem != null)
        {
            healthSystem.armorPoints -= armorBoostValue;
            Debug.Log($"{healthSystem.gameObject.name} lost {armorBoostValue} armor boost. Current armor: {healthSystem.armorPoints}");
        }
    }

    private bool IsArmorBoostActive(HealthSystem healthSystem)
    {
        return healthSystem.armorPoints >= armorBoostValue; // Prevent multiple boosts from stacking
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = boostZoneColor;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), radius);
    }
}
