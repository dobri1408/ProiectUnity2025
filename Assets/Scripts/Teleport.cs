using UnityEngine;

public class TeleportFrom : MonoBehaviour
{
    public Transform to;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (to == null)
        {
            Debug.LogError("TO destination not assigned!");
            return;
        }

        // Stop Rigidbody velocity if present
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
        // Remove visuals from this GameObject
        RemoveVisuals(gameObject);
    }

    void RemoveVisuals(GameObject obj)
    {
        if (obj == null) return;

        // Remove MeshRenderer if it exists
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) Destroy(mr);

        // Remove MeshFilter if it exists
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if (mf != null) Destroy(mf);

        // Remove SkinnedMeshRenderer if it exists (for characters)
        SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
        if (smr != null) Destroy(smr);
    }
}

