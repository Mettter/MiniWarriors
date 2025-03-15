using UnityEngine;
using System.Collections;

public class ShowObject : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation
    private bool isActivated = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isActivated)
        {
            StartCoroutine(ActivateWithDelay(1f)); // Start delay coroutine
            isActivated = true; // Prevents multiple activations
        }

        // Rotate the object continuously
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private IEnumerator ActivateWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.localScale = new Vector3(1f, 1f, transform.localScale.z);
    }
}
