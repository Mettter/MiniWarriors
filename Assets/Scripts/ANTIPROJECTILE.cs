using UnityEngine;
using System.Collections.Generic;

public class AntiProjectile : MonoBehaviour
{
    public GameObject effectObject; // Visual effect that follows the host
    public float detectionRadius = 2f; // Radius of the anti-projectile field
    public float yOffset = -0.25f; // Y offset for overlap circle and effect
    public int destroyCount = 5; // Number of projectiles that can be destroyed before deactivation
    
    private GameObject activeEffect;
    private bool isPKeyPressed = false;
    private bool isShieldActivated = false; // Instance of the effect object

    void Start()
    {
    }

    void Update()
    {
        if (activeEffect != null)
        {
            activeEffect.transform.position = GetEffectPosition(); // Keep effect attached to host
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPKeyPressed = true;
        }

        if (isPKeyPressed)
        {
            DetectAndDestroyProjectiles();
            if (!isShieldActivated)
            {
                isShieldActivated = true;
                Vector3 effectPosition = new Vector3(GetEffectPosition().x, GetEffectPosition().y, 0);
                activeEffect = Instantiate(effectObject, effectPosition, Quaternion.identity);
                ApplyEffectToNearestAlly();
            }
        }
    }

    void DetectAndDestroyProjectiles()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(GetDetectionPosition(), detectionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject && hit.gameObject.GetComponent<Projectile>() != null)
            {
                string enemyTag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
                if (hit.gameObject.CompareTag(enemyTag))
                {
                    Destroy(hit.gameObject);
                    destroyCount--;
                    
                    if (destroyCount <= 0)
                    {
                        Destroy(activeEffect);
                        Destroy(this);
                        return;
                    }
                }
            }
        }
    }

    void ApplyEffectToNearestAlly()
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        GameObject nearestAlly = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider2D ally in allies)
        {
            if (ally.gameObject == gameObject || ally.gameObject.GetComponent<Projectile>() != null)
                continue;

            if (ally.gameObject.CompareTag(gameObject.tag))
            {
                float distance = Vector2.Distance(transform.position, ally.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestAlly = ally.gameObject;
                }
            }
        }

        if (nearestAlly != null && nearestAlly.GetComponent<AntiProjectile>() == null)
        {
            AntiProjectile newEffect = nearestAlly.AddComponent<AntiProjectile>();
            newEffect.effectObject = effectObject;
            newEffect.detectionRadius = detectionRadius;
            newEffect.yOffset = yOffset;
            newEffect.destroyCount = destroyCount;
        }
    }

    Vector2 GetDetectionPosition()
    {
        return new Vector2(transform.position.x, transform.position.y + yOffset);
    }

    Vector2 GetEffectPosition()
    {
        return new Vector2(transform.position.x, transform.position.y + yOffset);
    }

    void OnDestroy()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect); // Destroy the effect object when the host is destroyed
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetDetectionPosition(), detectionRadius);
    }
}