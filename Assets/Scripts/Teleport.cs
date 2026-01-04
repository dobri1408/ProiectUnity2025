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
}

