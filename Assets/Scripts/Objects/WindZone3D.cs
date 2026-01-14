using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WindZone3D : MonoBehaviour
{
    // Wind direction constants
    private const float directionMagnitudeThreshold = 0.0001f;

    [Header("Wind Direction")]
    public bool useLocalDirection = true;
    public Vector3 direction = Vector3.right; // normalized internally

    [Header("Strength")]
    public float strength = 10f;              // m/s (CharacterController) or accel (Rigidbody)
    public float maxPushSpeed = 0f;           // 0 = no clamp, else caps wind contribution


    private Collider col;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        if (!col.isTrigger)
            Debug.LogWarning($"{name}: WindZone3D collider should be Trigger.");
    }

    // Calculates wind force at a given world position
    public Vector3 GetWindVector(Vector3 worldPosition)
    {
        Vector3 dir = (useLocalDirection ? transform.TransformDirection(direction) : direction);
        dir = dir.sqrMagnitude > directionMagnitudeThreshold ? dir.normalized : Vector3.zero;

        float mult = 1f;


        Vector3 wind = dir * (strength * mult);

        if (maxPushSpeed > 0f)
            wind = Vector3.ClampMagnitude(wind, maxPushSpeed);

        return wind;
    }

    // Registers wind receiver when it enters the zone
    private void OnTriggerEnter(Collider other)
    {
        var r = other.GetComponentInParent<WindReceiver3D>();
        if (r) r.AddZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var r = other.GetComponentInParent<WindReceiver3D>();
        if (r) r.RemoveZone(this);
    }
}
