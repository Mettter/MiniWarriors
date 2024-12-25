using UnityEngine;
using System.Collections;

public class DecayHealth : MonoBehaviour
{
    [Header("Decay Settings")] // Adds a header in the inspector for better organization
    [Tooltip("Amount of damage dealt per tick during decay.")]
    public float decayDamage = 5f;   // Damage dealt per tick

    [Tooltip("Time interval between each decay tick (in seconds).")]
    public float decaySpeed = 1f;   // Interval between ticks in seconds

    private bool isDecayTriggered = false; // Tracks if decay has been triggered
    private HealthSystem healthSystem; // Reference to the HealthSystem component
    private Coroutine decayCoroutine; // To manage the decay process

    private void Start()
    {
        // Attempt to get the HealthSystem component from the current object
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogWarning($"{gameObject.name} does not have a HealthSystem component. Decay will not work.");
        }
    }

    private void Update()
    {
        // Trigger decay when P is pressed, but only once
        if (Input.GetKeyDown(KeyCode.P) && !isDecayTriggered)
        {
            StartDecay();
            isDecayTriggered = true; // Ensure decay can only be triggered once
        }
    }

    /// <summary>
    /// Starts the decay process.
    /// </summary>
    private void StartDecay()
    {
        if (healthSystem == null)
        {
            Debug.LogWarning($"{gameObject.name} cannot decay because HealthSystem is missing.");
            return;
        }

        // Stop any ongoing decay before starting a new one
        if (decayCoroutine != null)
        {
            StopCoroutine(decayCoroutine);
        }

        decayCoroutine = StartCoroutine(DecayRoutine());
        Debug.Log($"{gameObject.name} has started decaying.");
    }

    /// <summary>
    /// The coroutine that deals decay damage over time.
    /// </summary>
    private IEnumerator DecayRoutine()
    {
        while (healthSystem.currentHealth > 0)
        {
            healthSystem.TakeDamage(decayDamage);
            Debug.Log($"{gameObject.name} is decaying. Took {decayDamage} damage. Remaining health: {healthSystem.currentHealth}");

            yield return new WaitForSeconds(decaySpeed);
        }

        Debug.Log($"{gameObject.name} has decayed to 0 health.");
        decayCoroutine = null; // Reset the coroutine reference
    }

    /// <summary>
    /// Stops the decay process.
    /// </summary>
    public void StopDecay()
    {
        if (decayCoroutine != null)
        {
            StopCoroutine(decayCoroutine);
            decayCoroutine = null;
            Debug.Log($"{gameObject.name} stopped decaying.");
        }
    }
}
