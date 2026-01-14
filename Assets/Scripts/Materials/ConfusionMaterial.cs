using System;
using UnityEngine;

// Material that inverts mouse sensitivity when grabbed/released
public class ConfusionMaterial : AbstractMaterial
{
    // Inverts mouse sensitivity on grab
    public override void grab(Hand hand, Player plr)
    {
        plr.mouseSens *= -1;
    }

    // Reverts mouse sensitivity on release
    public override void release(Hand hand, Player plr)
    {
        plr.mouseSens *= -1;
    }
}