using UnityEngine;

// Rotates the skybox based on time for dynamic sky effect
public class Skybox : MonoBehaviour
{
    private const float defaultSpeed = 0.13f;

    public float speed = defaultSpeed;

    // Applies rotation to skybox material each frame.
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}
