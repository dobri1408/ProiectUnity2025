using UnityEngine;

public class Hand : MonoBehaviour
{
    private Rigidbody rb;
    public bool isAnchored = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (Input.GetMouseButton(0))
            isAnchored = true;
    }

    void FixedUpdate()
    {
        
        if (isAnchored && Input.GetMouseButton(0))
            rb.linearVelocity = Vector3.zero;
        else {
            isAnchored = false;
        }   
        
    }
}
