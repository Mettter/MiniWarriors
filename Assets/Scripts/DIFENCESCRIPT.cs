using System.Collections;
using UnityEngine;

public class DIFENCESCRIPT : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private float spawnDelay = 1f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float smoothRotationSpeed = 5f;

    [Header("Overlap Circle Settings")]
    [SerializeField] private float overlapRadius = 5f;
    [SerializeField] private Color overlapColor = Color.blue;

    [Header("Armor Boost Settings")]
    [SerializeField] private int boostAmount = 50;
    [SerializeField] private int boostDuration = 10;

    private Quaternion targetRotation;
    private bool canAddArmor = true;
    private Transform selectedTarget = null; // Store the first found target
    private Vector3 teleportOffset = new Vector3(0f, -0.25f, 0f); // Y offset

    private void Start()
    {
        targetRotation = transform.rotation;
        StartCoroutine(SpawnLoop());

        // Select a target once at start
        SelectInitialTarget();
    }

    private void Update()
    {
        targetRotation *= Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smoothRotationSpeed);

        // Only teleport if a valid target exists
        if (selectedTarget != null)
        {
            transform.position = selectedTarget.position + teleportOffset; // Apply the offset
        }

        CheckForCollisions();
    }

    private void SelectInitialTarget()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, overlapRadius);
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in hitObjects)
        {
            GameObject obj = col.gameObject;

            if (obj.CompareTag(gameObject.tag) && obj.GetComponent<Projectile>() == null)
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    selectedTarget = obj.transform; // Store the first found target
                }
            }
        }
    }

    private void SpawnObject()
    {
        if (spawnObject != null)
        {
            GameObject newObject = Instantiate(spawnObject, transform.position, Quaternion.identity);
            newObject.tag = gameObject.tag;
        }
    }

    private void CheckForCollisions()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, overlapRadius);

        foreach (var hitObject in hitObjects)
        {
            if (hitObject.CompareTag(gameObject.tag))
            {
                Projectile otherProjectile = hitObject.GetComponent<Projectile>();
                if (otherProjectile == null)
                {
                    HealthSystem healthSystem = hitObject.GetComponent<HealthSystem>();
                    if (healthSystem != null && canAddArmor)
                    {
                        healthSystem.AddArmor(boostAmount, boostDuration);
                        StartCoroutine(ArmorCooldown());
                    }
                }
            }
        }
    }

    private IEnumerator ArmorCooldown()
    {
        canAddArmor = false;
        yield return new WaitForSeconds(1f);
        canAddArmor = true;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnObject();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = overlapColor;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}
