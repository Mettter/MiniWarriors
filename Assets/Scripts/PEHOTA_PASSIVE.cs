using System.Collections;
using UnityEngine;

public class PEHOTA_PASIVE : MonoBehaviour
{
    private HealthSystem healthSystem;
    public float healthAmount = 10f;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ApplyHealthBoost());
        }
    }

    IEnumerator ApplyHealthBoost()
    {
        yield return new WaitForSeconds(1f);

        string objTag = gameObject.tag; // Get this object's tag
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(objTag);

        int validTargets = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && obj.GetComponent<ROYAL>() && !obj.GetComponent<Projectile>())
            {
                validTargets++;
            }
        }

        float totalHealthIncrease = healthAmount * validTargets;

        if (healthSystem != null)
        {
            healthSystem.maxHealth += totalHealthIncrease;
            healthSystem.currentHealth += totalHealthIncrease;
        }
    }
}
