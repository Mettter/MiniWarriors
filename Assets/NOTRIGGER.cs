using UnityEngine;

public class ForceDefaultCollider : MonoBehaviour
{
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (col != null && col.isTrigger)
        {
            col.isTrigger = false; // Force it to be a solid collider
        }
    }
}
