using System;
using UnityEngine;

// Material that applies upward antigravity force when released
public class AntigravMaterial : AbstractMaterial
{
    private const float antigravPower = 20f;

    // No effect when grabbing antigrav material
    public override void grab(Hand hand, Player plr)
    {
        return;
    }

    // Applies upward force based on direction when released
    public override void release(Hand hand, Player plr)
    {
        Rigidbody rb = plr.GetComponent<Rigidbody>();

        Vector3 direction = plr.transform.position - hand.transform.position;

        rb.linearVelocity += direction.normalized * antigravPower;
    }
}