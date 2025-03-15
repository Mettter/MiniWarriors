using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capitan : MonoBehaviour
{
    public ManaSystem manaSystem;
    public int zalpMana;
    public GameObject cannonballPrefab;
    public List<Transform> firePositions;
    public float shootSpeed = 5f;
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;

    private void Start()
    {
        if (manaSystem == null)
        {
            manaSystem = GetComponent<ManaSystem>();
        }
    }

    private void Update()
    {
        if (manaSystem != null && manaSystem.currentMana >= zalpMana)
        {
            FireCannonballs();
            manaSystem.currentMana = 0; // Reset mana after firing
        }
    }

    private void FireCannonballs()
    {
        int direction = GetShootDirection();
        
        foreach (Transform firePos in firePositions)
        {
            GameObject cannonball = Instantiate(cannonballPrefab, firePos.position, Quaternion.identity);
            cannonball.tag = gameObject.tag; // Give the cannonball the same tag as the host
            
            StartCoroutine(DashObject(cannonball, direction));
        }
    }

    private int GetShootDirection()
    {
        if (CompareTag("Team1")) return 1;  // Team1 shoots right
        if (CompareTag("Team2")) return -1; // Team2 shoots left
        return 0;
    }

    private IEnumerator DashObject(GameObject obj, int direction)
{
    Vector2 originalPosition = obj.transform.position;
    Vector2 targetPosition = originalPosition + new Vector2(dashDistance * direction, 0);

    float elapsed = 0f;

    while (elapsed < dashDuration)
    {
        obj.transform.position = Vector2.Lerp(originalPosition, targetPosition, elapsed / dashDuration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    obj.transform.position = targetPosition;

    // Destroy the cannonball after dash completes
    Destroy(obj);
}

}
