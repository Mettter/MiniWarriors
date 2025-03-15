using UnityEngine;

public class SKELETISATION : MonoBehaviour
{
    public GameObject skeletonPrefab; // The skeleton prefab to spawn
    public string skeletonTeam;       // The team that the spawned skeleton belongs to

    private void OnDestroy()
    {
        if (skeletonPrefab != null)
        {
            // Adjust spawn position with a Y offset
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
            
            // Instantiate the skeletonPrefab at the new position
            GameObject skeleton = Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);

            // Apply the correct team tag to the skeleton
            if (!string.IsNullOrEmpty(skeletonTeam))
            {
                skeleton.tag = skeletonTeam; // Set the tag to the caster's team
            }
        }
    }
}
