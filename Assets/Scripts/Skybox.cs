using UnityEngine;

public class Skybox : MonoBehaviour
{
    public float speed = 0.13f;

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}
