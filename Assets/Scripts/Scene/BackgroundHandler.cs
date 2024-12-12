using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public Transform[] layers;         // Array of parallax layers
    public float[] parallaxFactors;    // Parallax factors for each layer
    public float smoothing = 1f;       // Smoothing factor for parallax movement

    private float previousPlayerPositionX; // Store the previous x position of the player

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned!");
            return;
        }

        previousPlayerPositionX = playerTransform.position.x;
    }

    void Update()
    {
        // Calculate the parallax effect for each layer based on the player's movement
        float playerMovement = previousPlayerPositionX - playerTransform.position.x;

        for (int i = 0; i < layers.Length; i++)
        {
            float parallax = playerMovement * parallaxFactors[i];

            // Calculate the target position for the layer
            float targetX = layers[i].position.x + parallax;
            Vector3 targetPosition = new Vector3(targetX, layers[i].position.y, layers[i].position.z);

            // Smoothly move the layer towards the target position
            layers[i].position = Vector3.Lerp(layers[i].position, targetPosition, smoothing * Time.deltaTime);
        }

        // Update the previous player position
        previousPlayerPositionX = playerTransform.position.x;
    }
}