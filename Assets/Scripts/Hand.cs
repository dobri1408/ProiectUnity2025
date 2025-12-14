using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    private Rigidbody rb;
    public bool isAnchored = false;
    private GameObject collObj; // object hand is anchored to, used for moving platforms
    private Transform tracker; // position that follows platform
    public Transform player;

    private HashSet<string> grabable = new HashSet<string>()
    {
        "Brick",
        "Wood",
        "StoneBrick",
        "Rock",
        "TestMat",
        "MossBrick"
    };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (Input.GetMouseButton(0) && IsWithinHandDistance() && isGrabableMat(collision.gameObject) && !isAnchored) {
            isAnchored = true;

            collObj = collision.gameObject;
            tracker = new GameObject("Tracker").transform;
            tracker.SetParent(collObj.transform);
            tracker.position = transform.position;
        }
    }

    private bool IsWithinHandDistance()
    {
        if (player == null) return false;
        
        float distance = Vector3.Distance(transform.position, player.position);
        Player p = player.GetComponent<Player>();
        
        if (p != null)
        {
            return distance <= p.handDist * 1.1f;
        }
        return false;
    }

    private bool isGrabableMat(GameObject obj)
    {
        if (obj == null) return false;
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return false;
        
        // Check all materials on the object
        foreach (Material mat in renderer.sharedMaterials)
        {
            if (mat == null) continue;
            
            string matName = mat.name.Replace(" (Instance)", "");
            
            if (grabable.Contains(matName))
                return true;
        }
        
        return false;
    }

    void FixedUpdate()
    {
        if(!IsWithinHandDistance()) isAnchored = false;
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
            if (tracker != null) Destroy(tracker.gameObject);
            isAnchored = false;
            collObj = null; // free reference and tracker
            tracker = null;
        }   
        
    }
}
