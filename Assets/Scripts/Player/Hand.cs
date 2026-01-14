using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;

public class Hand : MonoBehaviour
{
    public float grabableDistanceMultiplier = 1.25f; // how far until forcefully let go
    public bool isAnchored = false; // Main state to decide if hand is free or holding
    private string heldMat; // material currently being held

    private Rigidbody rb;
    private GameObject collObj; // object hand is anchored to, used for moving platforms
    private Transform tracker; // position that follows platform
    public Transform player;
    private Player playerObj;

    private Dictionary<string, AbstractMaterial> matMap;

    // list of grabable materials, might replace with material class
    private HashSet<string> grabable = new HashSet<string>()
    {
        "Brick",
        "Wood",
        "StoneBrick",
        "Rock",
        "TestMat",
        "Confusion",
        "Antigrav",
    };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerObj = transform.parent.GetComponentInChildren<Player>(); 
    }

    string getMaterialName(GameObject obj)
    {
        if (obj == null) return "";

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return "";

        Material[] materials = renderer.sharedMaterials;
        if (materials == null || materials.Length == 0) return "";

        Material mat = materials[0];
        if (mat == null) return "";

        return mat.name;
    }

    // Set grab status, call according material functionality
    void setAnchored(bool state)
    {
        if( isAnchored == state ) return;
        isAnchored = state;

        if( heldMat == "" ) return;
        if( !matMap.ContainsKey(heldMat) ) return;


        if( isAnchored ) matMap[heldMat].grab(this, playerObj);
        else matMap[heldMat].release(this, playerObj);
    }

    void OnCollisionStay(Collision collision)
    {
        // if player is holding and is able to hold, create a tracker for mouse position
        if (Input.GetMouseButton(0) && IsWithinHandDistance() && isGrabableMat(collision.gameObject) && !isAnchored && playerObj.exhausted == false) {
            heldMat = getMaterialName(collision.gameObject);
            if(!isAnchored) setAnchored(true);

            collObj = collision.gameObject;
            tracker = new GameObject("Tracker").transform;
            tracker.SetParent(collObj.transform);
            tracker.position = transform.position;
        }
    }

    // if hand is too far to hold
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

    // checks for material name in hashset
    private bool isGrabableMat(GameObject obj)
    {
        string mat = getMaterialName(obj);

        string matName = mat.Replace(" (Instance)", "");

        return grabable.Contains(matName);
    }

    void Awake()
    {
        // initialize materials
        matMap = new Dictionary<string, AbstractMaterial>();
        matMap["Confusion"] = gameObject.AddComponent<ConfusionMaterial>();
        matMap["Antigrav"] = gameObject.AddComponent<AntigravMaterial>();
    }

    void FixedUpdate()
    {
        if(!IsWithinHandDistance()) setAnchored(false);
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
            if( isAnchored ) setAnchored(false);
            collObj = null; // free reference and tracker
            tracker = null;
        }   
        
    }
}