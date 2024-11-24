using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;  // Tracks if the object is being dragged
    private Vector3 offset;          // Stores the offset between the mouse and the object position
    private Camera mainCamera;       // Reference to the main camera
    [SerializeField] public bool isDragEnabled = true; // Global flag to enable/disable dragging

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    private void Update()
    {
        // Toggle dragging with the P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            isDragEnabled = !isDragEnabled;

            // If dragging is disabled, stop dragging immediately
            if (!isDragEnabled && isDragging)
            {
                isDragging = false;
                return; // Exit the update function immediately to stop dragging.
            }
        }

        // If dragging is enabled, update position
        if (isDragging && isDragEnabled)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private void OnMouseDown()
    {
        // Start dragging only if dragging is enabled
        if (isDragEnabled)
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
        }
    }

    private void OnMouseUp()
    {
        // Stop dragging when the mouse is released
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in world space
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the object collides with something tagged 'Barrier', stop dragging
        if (collision.gameObject.CompareTag("Barrier"))
        {
            isDragging = false;
        }
    }
}
