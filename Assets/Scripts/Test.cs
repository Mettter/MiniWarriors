using UnityEngine;

public class DeskSwapper : MonoBehaviour
{
    public GameObject desk1Object;
    public GameObject desk2Object;

    private void OnMouseDown()
    {
        // Check if the host clicked on the desk
        if (desk1Object.activeSelf)
        {
            // If desk1 is active, deactivate it and activate desk2
            desk1Object.SetActive(false);
            desk2Object.SetActive(true);
        }
        else if (desk2Object.activeSelf)
        {
            // If desk2 is active, deactivate it and activate desk1
            desk2Object.SetActive(false);
            desk1Object.SetActive(true);
        }
    }
}
