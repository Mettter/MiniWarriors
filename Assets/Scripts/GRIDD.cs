using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;  // Tracks if the object is being dragged
    private Vector3 offset;           // Stores the offset between the mouse and the object position
    private Camera mainCamera;        // Reference to the main camera
    [SerializeField] public bool isDragEnabled = true; // Global flag to enable/disable dragging

    private static GameObject selectedObject = null; // Stores the first clicked object for teleportation
    private bool SelectMode = true;
    private bool pKeyPressed = false; // Selection mode is enabled by default
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] private float yOffset = 0.5f; // Offset applied when teleporting an object

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        // Toggle selection mode with the S key
        if (Input.GetKeyDown(KeyCode.S))
        {
            SelectMode = !SelectMode; // Toggle selection mode
            Debug.Log("Selection Mode: " + (SelectMode ? "Enabled" : "Disabled"));

            // Reset selection when switching to drag mode
            if (!SelectMode && selectedObject != null)
            {
                selectedObject.GetComponent<SpriteRenderer>().color = selectedObject.GetComponent<DragAndDrop>().originalColor; // Reset color
                selectedObject = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            pKeyPressed = true;
        }

        // If in selection mode, teleport selected object to mouse position with Space key
        if (SelectMode && selectedObject != null && Input.GetKeyDown(KeyCode.Space))
        {
            TeleportSelectedObjectToMouse();
        }

        // If dragging is enabled and not in selection mode, update position
        if (isDragging && !SelectMode && isDragEnabled)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private void OnMouseDown()
    {
        if (!pKeyPressed)
        {
            if (SelectMode)
            {
                // In selection mode, select the object on click
                if (selectedObject == null || selectedObject != gameObject)
                {
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<SpriteRenderer>().color = selectedObject.GetComponent<DragAndDrop>().originalColor; // Reset previous selection
                    }
                    selectedObject = gameObject; // Store the first clicked object

                    // Highlight selected object based on its tag
                    if (selectedObject.CompareTag("Team2"))
                    {
                        spriteRenderer.color = Color.red; // Red for Team2
                    }
                    else
                    {
                        spriteRenderer.color = Color.green; // Green for others
                    }

                    Debug.Log("Object Selected: " + gameObject.name);
                }
            }
            else
            {
                // In drag mode, start dragging if not in selection mode
                if (isDragEnabled)
                {
                    isDragging = true;
                    offset = transform.position - GetMouseWorldPosition();
                }
            }
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
        mouseScreenPosition.z = 0f; // Fix for 2D games (no depth)
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void TeleportSelectedObjectToMouse()
    {
        if (selectedObject != null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            selectedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y + yOffset, selectedObject.transform.position.z);
            selectedObject.GetComponent<SpriteRenderer>().color = selectedObject.GetComponent<DragAndDrop>().originalColor; // Reset color
            selectedObject = null; // Reset selection after teleport
            Debug.Log("Object Teleported to Mouse Position: " + mousePosition);
        }
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
