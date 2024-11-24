using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour
{
    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
