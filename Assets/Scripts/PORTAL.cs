using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    public bool isFirstPortal = true;
    public float offsetAmount = 5f;
    public float tpPositionOffsetY = 1f; // Y position offset after teleporting
    public Vector2 detectionSize = new Vector2(2f, 2f); // Square detection area
    public string enemySideTag = "enemySide"; // Tag for reversing offset

    private Portal linkedPortal;
    private HashSet<Transform> teleportedObjects = new HashSet<Transform>(); // Stores objects that have teleported

    void Start()
    {
        if (isFirstPortal)
        {
            SpawnLinkedPortal();
        }
    }

    void SpawnLinkedPortal()
    {
        float finalOffset = offsetAmount;

        // Check if touching an "enemySide" object (by tag)
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(enemySideTag))
            {
                finalOffset = -offsetAmount;
                break; // Stop checking after finding one
            }
        }

        // Create new portal
        GameObject newPortal = Instantiate(gameObject, new Vector3(transform.position.x + finalOffset, transform.position.y, transform.position.z), Quaternion.identity);
        Portal portalScript = newPortal.GetComponent<Portal>();
        portalScript.isFirstPortal = false;
        linkedPortal = portalScript;
        portalScript.linkedPortal = this;
    }

    void Update()
    {
        DetectObjects();
    }

    void DetectObjects()
    {
        Collider2D[] objects = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0); // Detect all objects

        foreach (Collider2D obj in objects)
        {
            // Skip objects with the Projectile component
            if (obj.GetComponent<Projectile>() != null) continue;

            // Skip objects that have already teleported once
            if (teleportedObjects.Contains(obj.transform)) continue;

            // Only teleport if it has the same tag as the portal
            if (obj.CompareTag(gameObject.tag))
            {
                Teleport(obj.transform);
            }
        }
    }

    void Teleport(Transform target)
    {
        if (linkedPortal)
        {
            // Move to the center of the other portal + apply Y offset
            target.position = new Vector3(linkedPortal.transform.position.x, linkedPortal.transform.position.y + tpPositionOffsetY, target.position.z);

            // Mark object as permanently teleported
            linkedPortal.teleportedObjects.Add(target);
            teleportedObjects.Add(target);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, detectionSize);
    }
}
