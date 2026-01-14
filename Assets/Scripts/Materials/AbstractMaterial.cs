using UnityEngine;

public class AbstractMaterial : MonoBehaviour
{
    public virtual void grab(Hand hand, Player plr)
    {
        Debug.Log("Grabbed mat");
    }

    public virtual void release(Hand hand, Player plr)
    {
        Debug.Log("Released mat");
    }
}
