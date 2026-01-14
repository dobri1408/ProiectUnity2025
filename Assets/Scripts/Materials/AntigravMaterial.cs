using System;
using UnityEngine;

public class AntigravMaterial : AbstractMaterial
{
    private float power = 20f;
    public override void grab(Hand hand, Player plr)
    {
        return;
    }

    public override void release(Hand hand, Player plr)
    {
        Rigidbody rb = plr.GetComponent<Rigidbody>();

        Vector3 direction = plr.transform.position - hand.transform.position;

        rb.linearVelocity += direction.normalized * power;
    }
}