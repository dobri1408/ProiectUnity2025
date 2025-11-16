using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float moveDampPerc = 0.98f;
    public float maxSpeed = 5f;

    [Header("Camera Settings")]
    private Transform camTransform;
    public float mouseSens = 300f;
    private float camX;

    [Header("Hand Settings")]
    public Rigidbody hand;
    public float handSpeed = 5f;
    public float handDist = 2f;
    public float handDamp = 0.2f;

    private Rigidbody handRB;
    private Hand handObj;
    private Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        camTransform = GetComponentInChildren<Camera>().transform;
        handObj = hand.GetComponent<Hand>();
    }

    void Update()
    {
        HandleMouse();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleHand();
    }

    void HandleMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        camX -= mouseY;
        camX = Mathf.Clamp(camX, -90f, 90f);
        camTransform.localRotation = Quaternion.Euler(camX, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (handObj.isAnchored || (rb.linearVelocity.magnitude >= maxSpeed))
        {
            h = 0;
            v = 0;
        }

        Vector3 moveDir = transform.forward * v + transform.right * h;
        Vector3 force = moveDir * moveSpeed;

        rb.AddForce(force);
        rb.linearVelocity *= moveDampPerc;
    }

    void HandleHand()
    {
        if (!handObj.isAnchored)
        {
            Vector3 handPos = transform.position + camTransform.forward * handDist;
            Vector3 target = handPos - hand.transform.position;
            hand.linearVelocity = Vector3.Lerp(hand.linearVelocity, target * 10, handDamp);
        }
        else //otherwise move player
        {
            Vector3 playerPos = hand.transform.position + camTransform.forward * -handDist;
            Vector3 target = playerPos - transform.position;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target * 10, handDamp);
        }
    }
}
