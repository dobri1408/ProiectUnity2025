using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    public float grabableDistanceMultiplier = 1.25f; // how far until forcefully let go
    public bool isAnchored = false;

    private Rigidbody rb;
    private GameObject collObj; // object hand is anchored to, used for moving platforms
    private Transform tracker; // position that follows platform
    public Transform player;
    private Player playerObj;

    // list of grabable materials, might replace with material class
    private HashSet<string> grabable = new HashSet<string>()
    {
        "Brick",
        "Wood",
        "StoneBrick",
        "Rock",
        "TestMat",
    };

    // Initializes hand component and finds player reference.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerObj = transform.parent.GetComponentInChildren<Player>(); 
    }

    // Handles grabbing to climbable surfaces when mouse button is held and conditions are met.
    void OnCollisionStay(Collision collision)
    {
        // if player is holding and is able to hold, create a tracker for mouse position
        if (Input.GetMouseButton(0) && IsWithinHandDistance() && isGrabableMat(collision.gameObject) && !isAnchored && playerObj.exhausted == false) {
            isAnchored = true;

            collObj = collision.gameObject;
            tracker = new GameObject("Tracker").transform;
            tracker.SetParent(collObj.transform);
            tracker.position = transform.position;
        }
    }

    // Checks if hand is within valid grabbing distance from player.
    private bool IsWithinHandDistance()
    {
        if (player == null) return false;
        
        float distance = Vector3.Distance(transform.position, player.position);
        Player p = player.GetComponent<Player>();
        
        if (p != null)
        {
            return distance <= p.handDist * grabableDistanceMultiplier;
        }
        return false;
    }

    // Checks if a game object has a climbable material.
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
        if (isAnchored && Input.GetMouseButton(0) && playerObj.exhausted == false)
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