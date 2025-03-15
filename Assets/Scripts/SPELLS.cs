using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed of movement

    private Vector3 moveDirection;
    private bool isTeam1;  // Flag to check if the object is part of Team1
    private bool isTeam2;  // Flag to check if the object is part of Team2
    private bool pKeyPressed = false;  // Flag to allow movement after "P" key is pressed

    public GameObject[] team1Objects = new GameObject[5];  // Array to store 5 Team1 objects
    public GameObject[] team2Objects = new GameObject[5];  // Array to store 5 Team2 objects

    public bool[] isNeToDe1Team1 = new bool[5];  // Flags to check if Team1 objects need to be destroyed
    public bool[] isNeToDe1Team2 = new bool[5];  // Flags to check if Team2 objects need to be destroyed

    void Start()
    {
        // Check once during start to see if the object has Team1S or Team2S script
        isTeam1 = GetComponent<Team1S>() != null;
        isTeam2 = GetComponent<Team2S>() != null;
    }

    void Update()
    {
        // Check if the "P" key is pressed and set pKeyPressed to true if it hasn't been set already
        if (!pKeyPressed && Input.GetKeyDown(KeyCode.P))
        {
            pKeyPressed = true;
        }

        // If pKeyPressed is false, skip all logic
        if (!pKeyPressed)
        {
            return;
        }

        HandleMovement();
        HandleTeleportation();
    }

    private void HandleMovement()
    {
        // Initialize moveDirection as zero
        moveDirection = Vector3.zero;

        // Handle movement based on the tag of the object
        if (isTeam1)  // If the object is Team1
        {
            // WASD controls for Team1
            if (Input.GetKey(KeyCode.W))  // Move up
                moveDirection += Vector3.up;

            if (Input.GetKey(KeyCode.S))  // Move down
                moveDirection += Vector3.down;

            if (Input.GetKey(KeyCode.A))  // Move left
                moveDirection += Vector3.left;

            if (Input.GetKey(KeyCode.D))  // Move right
                moveDirection += Vector3.right;
        }
        else if (isTeam2)  // If the object is Team2
        {
            // Arrow keys controls for Team2
            if (Input.GetKey(KeyCode.UpArrow))  // Move up
                moveDirection += Vector3.up;

            if (Input.GetKey(KeyCode.DownArrow))  // Move down
                moveDirection += Vector3.down;

            if (Input.GetKey(KeyCode.LeftArrow))  // Move left
                moveDirection += Vector3.left;

            if (Input.GetKey(KeyCode.RightArrow))  // Move right
                moveDirection += Vector3.right;
        }

        // Normalize the direction to avoid faster diagonal movement
        if (moveDirection.magnitude > 1)
            moveDirection.Normalize();

        // Apply movement
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDirection, moveSpeed * Time.deltaTime);
    }

    private void HandleTeleportation()
    {
        // Handle teleportation for Team1 using keys 1-5
        if (isTeam1)
        {
            for (int i = 0; i < 5; i++)
            {
                KeyCode key = KeyCode.Alpha1 + i;

                if (Input.GetKeyDown(key))
                {
                    if (team1Objects[i] != null)
                    {
                        // Check if the object is a prefab
                        if (PrefabIsPresent(team1Objects[i]))
                        {
                            // Instantiate the object at the position
                            Instantiate(team1Objects[i], transform.position, Quaternion.identity);
                        }
                        else
                        {
                            // Teleport the object if it's not a prefab
                            team1Objects[i].transform.position = transform.position;
                        }

                        // Destroy other objects
                        for (int j = 0; j < team1Objects.Length; j++)
                        {
                            if (j != i && team1Objects[j] != null)
                            {
                                Destroy(team1Objects[j]);
                            }
                        }

                        // Destroy this object
                        Destroy(gameObject);

                        // Check if the teleported object needs to be destroyed
                        if (isNeToDe1Team1[i])
                        {
                            Destroy(team1Objects[i], 0.05f);  // Destroy after 0.2 seconds
                        }
                        break;
                    }
                }
            }
        }

        // Handle teleportation for Team2 using keys H, J, K, N, M
        if (isTeam2)
        {
            KeyCode[] team2Keys = { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.N, KeyCode.M };

            for (int i = 0; i < team2Keys.Length; i++)
            {
                if (Input.GetKeyDown(team2Keys[i]))
                {
                    if (team2Objects[i] != null)
                    {
                        // Check if the object is a prefab
                        if (PrefabIsPresent(team2Objects[i]))
                        {
                            // Instantiate the object at the position
                            Instantiate(team2Objects[i], transform.position, Quaternion.identity);
                        }
                        else
                        {
                            // Teleport the object if it's not a prefab
                            team2Objects[i].transform.position = transform.position;
                        }

                        // Destroy other objects
                        for (int j = 0; j < team2Objects.Length; j++)
                        {
                            if (j != i && team2Objects[j] != null)
                            {
                                Destroy(team2Objects[j]);
                            }
                        }

                        // Destroy this object
                        Destroy(gameObject);

                        // Check if the teleported object needs to be destroyed
                        if (isNeToDe1Team2[i])
                        {
                            Destroy(team2Objects[i], 0.05f);  // Destroy after 0.2 seconds
                        }
                        break;
                    }
                }
            }
        }
    }

    // Helper method to check if an object is a prefab
    private bool PrefabIsPresent(GameObject obj)
    {
        // Check if the object is a prefab by comparing its scene status
        return !obj.scene.isLoaded;
    }
}