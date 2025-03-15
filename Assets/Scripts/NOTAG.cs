using UnityEngine;

public class SetTagToNoTag : MonoBehaviour
{
    private void Update()
    {
        gameObject.tag = "NOTAG";  // Constantly sets the object's tag to "NOTAG"
    }
}
