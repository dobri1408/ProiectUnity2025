using UnityEngine;

public class TeleportFrom : MonoBehaviour
{
    public Transform to; // point to teleport player to

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
        RemoveVisuals(gameObject);
    }

    // Removes visuals from teleporter and target obj
    void RemoveVisuals(GameObject obj)
    {
        if (obj == null) return;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) Destroy(mr);

        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if (mf != null) Destroy(mf);

        SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
        if (smr != null) Destroy(smr);
    }
}

