using UnityEngine;

// Abstract base class for material effects when grabbed/released
public class AbstractMaterial : MonoBehaviour
{
    // Called when the player grabs this material
    public virtual void grab(Hand hand, Player plr)
    {
        Debug.Log("Grabbed mat");
    }

    // Called when the player releases this material
    public virtual void release(Hand hand, Player plr)
    {
        Debug.Log("Released mat");
    }
}
