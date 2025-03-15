using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
    private Animator animator;

    // Speed multiplier, can be adjusted in the Inspector
    public float animationSpeed = 1f;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Check if the Animator is found
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Update the animation speed based on the value of animationSpeed
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }

        // Optional: Use keyboard input to modify speed at runtime
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Increase speed
        {
            animationSpeed += 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) // Decrease speed
        {
            animationSpeed = Mathf.Max(0.1f, animationSpeed - 0.5f); // Ensure it doesn't go below 0.1
        }
    }
}
