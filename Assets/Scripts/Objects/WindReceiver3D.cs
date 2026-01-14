using System.Collections.Generic;
using UnityEngine;

public class WindReceiver3D : MonoBehaviour
{
    [Header("Blend")]
    public float enterBlend = 10f;   // higher = snaps faster
    public float exitBlend = 10f;

    [Header("Force Behavior")]
    public bool ignoreVertical = false;     // common for “side wind”
    public bool useFixedUpdateForRigidbody = true;

    private readonly List<WindZone3D> zones = new();
    private Vector3 targetWind;
    private Vector3 currentWind;

    private Rigidbody rb;
    private CharacterController cc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    public void AddZone(WindZone3D zone)
    {
        if (!zones.Contains(zone)) zones.Add(zone);
    }

    public void RemoveZone(WindZone3D zone)
    {
        zones.Remove(zone);
    }

    void Update()
    {
        if (rb && useFixedUpdateForRigidbody) return; // let FixedUpdate handle rb

        RecalculateTargetWind();
        BlendWind(Time.deltaTime);

        ApplyWind(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!rb || !useFixedUpdateForRigidbody) return;

        RecalculateTargetWind();
        BlendWind(Time.fixedDeltaTime);

        ApplyWind(Time.fixedDeltaTime);
    }

    private void RecalculateTargetWind()
    {
        Vector3 sum = Vector3.zero;
        for (int i = zones.Count - 1; i >= 0; i--)
        {
            if (zones[i] == null) { zones.RemoveAt(i); continue; }
            sum += zones[i].GetWindVector(transform.position);
        }

        if (ignoreVertical) sum.y = 0f;
        targetWind = sum;
    }

    private void BlendWind(float dt)
    {
        float blend = (targetWind.sqrMagnitude > 0.001f) ? enterBlend : exitBlend;
        // exponential smoothing
        float k = 1f - Mathf.Exp(-blend * dt);
        currentWind = Vector3.Lerp(currentWind, targetWind, k);
    }

    private void ApplyWind(float dt)
    {
        if (rb)
        {
            // Treat "currentWind" as acceleration-like push (consistent feel)
            rb.AddForce(currentWind, ForceMode.Acceleration);
        }
        else if (cc)
        {
            // Additive displacement (doesn't require modifying your player script)
            cc.Move(currentWind * dt);
        }
        else
        {
            // Fallback: directly move transform
            transform.position += currentWind * dt;
        }
    }
}
