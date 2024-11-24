using UnityEngine;

public class DestroyOnKeyPress : MonoBehaviour
{
    private void Update()
    {
        // Check if the P key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            Destroy(gameObject); // Destroy the game object this script is attached to
            Debug.Log($"{gameObject.name} has been destroyed."); // Optional log for debugging
        }
    }
}
