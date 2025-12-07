using UnityEngine;

public class Hand : MonoBehaviour
{
    private Rigidbody rb;
    public bool isAnchored = false;
    private GameObject collObj; // object hand is anchored to, used for moving platforms
    private Transform tracker; // position that follows platform

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (Input.GetMouseButton(0)) {
            isAnchored = true;

            collObj = collision.gameObject;
            tracker = new GameObject("Tracker").transform;
            tracker.SetParent(collObj.transform);
            tracker.position = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (isAnchored && Input.GetMouseButton(0))
        {
            rb.linearVelocity = Vector3.zero;
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            transform.position = tracker.position;
            transform.rotation = tracker.rotation;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white; // reset color
            isAnchored = false;
            collObj = null; // free reference and tracker
            tracker = null;
        }   
        
    }
}
