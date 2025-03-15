using UnityEngine;

public class WallScript : MonoBehaviour
{
    public float speed = 5f; // Speed of movement
    private string parentTag; // Store parent's tag

    void Start()
    {
        // Get and store parent's tag
        if (transform.parent != null)
        {
            parentTag = transform.parent.tag;
        }
    }

    void Update()
    {
        // Move right if tagged Team1, left if tagged Team2
        if (CompareTag("Team1"))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        else if (CompareTag("Team2"))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collision if the object has the same tag as the parent
        if (other.CompareTag(parentTag))
        {
            return;
        }

        // Destroy if colliding with the correct stopObject based on parent tag
        if (parentTag == "Team1" && other.CompareTag("stopObject"))
        {
            Destroy(gameObject);
        }
        else if (parentTag == "Team2" && other.CompareTag("stopObject2"))
        {
            Destroy(gameObject);
        }
    }
}
