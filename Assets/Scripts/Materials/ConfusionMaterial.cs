using System;
using UnityEngine;

public class ConfusionMaterial : AbstractMaterial
{
    public override void grab(Hand hand, Player plr)
    {
        plr.mouseSens *= -1;
    }

    public override void release(Hand hand, Player plr)
    {
        plr.mouseSens *= -1;
    }
}