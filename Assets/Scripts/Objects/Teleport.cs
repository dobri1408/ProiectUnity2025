using UnityEngine;

public class TeleportFrom : MonoBehaviour
{
    public Transform to; // point to teleport player to

    // Cache mesh components for removal in Start() to avoid repeated GetComponent calls
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) // only handle player, hand will be teleported by player logic
            return;

        if (to == null)
        {
            Debug.LogError("Missing teleport destination.");
            return;
        }

        // Zero player's velocity
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleport
        other.transform.position = to.position;
    }

    void Start()
    {
        // Cache all mesh components
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        RemoveVisuals();
    }

    // Removes visuals from teleporter by destroying cached mesh components
    void RemoveVisuals()
    {
        if (meshRenderer != null) Destroy(meshRenderer);
        if (meshFilter != null) Destroy(meshFilter);
        if (skinnedMeshRenderer != null) Destroy(skinnedMeshRenderer);
    }
}
