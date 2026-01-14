using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Physics constants
    private const float groundedVelocityThreshold = 0.5f;
    private const float groundCheckDistanceMultiplier = 1.5f;
    private const float cameraClampMin = -90f;
    private const float cameraClampMax = 90f;
    private const float spatialBlend2D = 0f;

    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public float airMoveSpeedMultiplier = 0.2f;
    public float linearDamp = 0.97f;
    public float maxSpeed = 6f;

    [Header("Camera Settings")]
    private Transform camTransform;
    public float mouseSens = 300f;
    private float camX;

    [Header("Hand Settings")]
    public Rigidbody hand;
    public float handSpeed = 5f;
    public float handDist = 2f;
    public float maxHandDist = 3.5f;
    public float handDamp = 0.7f;
    public float momentumBoost = 0.5f; // how much of running speed is added to swing
    [Header("Stamina")]
    public float maxStamina = 1f;
    private float staminaRegenMult = 0.35f;
    private float staminaDrainMult = 0.25f;
    public float stamina;
    public bool exhausted = false;

    [Header("Audio Settings")]
    public AudioClip windSound;
    public AudioClip footstepsSound;
    public float maxWindVolume = 1.0f;
    public float windVelocityThreshold = 0.3f;
    public float maxWindVelocity = 6f;
    public float groundCheckDistance = 0.67f;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;
    public float footstepVolume = 0.9f;

    private Rigidbody handRB;
    private Hand handObj;
    private Rigidbody rb;
    private AudioSource windAudioSource;
    private AudioSource footstepsAudioSource;
    private bool isGrounded;
    private float footstepTimer;
    private int layerMask; // Cached layer mask for ground checks
    public bool cheated = false;

    // Momentum grab
    private Vector3 velocityBeforeGrab;
    private bool wasAnchored = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        camTransform = GetComponentInChildren<Camera>().transform;
        handObj = hand.GetComponent<Hand>();

        handObj.player = transform; // give reference to self in hand

        // Cache layer mask to avoid recalculating in FixedUpdate
        layerMask = ~LayerMask.GetMask("Player");

        // Load saved mouse sensitivity
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSens = PlayerPrefs.GetFloat("MouseSensitivity");
        }

        // Setup audio sources
        windAudioSource = gameObject.AddComponent<AudioSource>();
        windAudioSource.clip = windSound;
        windAudioSource.loop = true;
        windAudioSource.volume = 0f;
        windAudioSource.playOnAwake = false;
        windAudioSource.spatialBlend = spatialBlend2D;

        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        footstepsAudioSource.clip = footstepsSound;
        footstepsAudioSource.loop = true;
        footstepsAudioSource.volume = 0f;
        footstepsAudioSource.playOnAwake = false;
        footstepsAudioSource.spatialBlend = spatialBlend2D;

        // Check if audio clips are assigned
        if (windSound != null)
        {
            windAudioSource.Play();
            Debug.Log("Wind audio started");
        }
        else
        {
            Debug.LogWarning("Wind sound clip not assigned!");
        }

        if (footstepsSound != null)
        {
            footstepsAudioSource.Play();
            Debug.Log("Footsteps audio started");
        }
        else
        {
            Debug.LogWarning("Footsteps sound clip not assigned!");
        }
    }

    void Update()
    {
        HandleMouse();
        HandleAudio();
    }

    // FixedUpdate is called for physics calculations. Handles movement, hand positioning, and stamina.
    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleHand();
        HandleStamina(); // call after hand for Anchored status
    }

    // Checks if the player is grounded using raycast and sphere cast for better accuracy.
    void CheckGrounded()
    {
        RaycastHit hit;
        Vector3 spherePosition = transform.position;
        
        // Use cached layer mask instead of recalculating
        
        // Check with raycast first
        bool rayHit = Physics.Raycast(spherePosition, Vector3.down, out hit, groundCheckDistance, layerMask);
        // Also check with sphere cast for better detection
        bool sphereHit = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, layerMask);
        isGrounded = rayHit || sphereHit;
        
        // Additional check
        if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) < groundedVelocityThreshold)
        {
            isGrounded = Physics.Raycast(spherePosition, Vector3.down, groundCheckDistance * groundCheckDistanceMultiplier, layerMask);
        }
    }

    // Handles camera rotation based on mouse input.
    void HandleMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        camX -= mouseY;
        camX = Mathf.Clamp(camX, cameraClampMin, cameraClampMax);
        camTransform.localRotation = Quaternion.Euler(camX, 0f, 0f);
    }

    // Handles player movement based on input. Applies force, damping, and speed limits.
    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = transform.forward * v + transform.right * h;

        // Only check max speed along the move direction
        if (handObj.isAnchored || (moveDir.sqrMagnitude > 0f && Vector3.Dot(rb.linearVelocity, moveDir.normalized) >= maxSpeed))
        {
            h = 0;
            v = 0;
            moveDir = Vector3.zero;
        }

        Vector3 force = moveDir * moveSpeed;
        if(!isGrounded) force *= airMoveSpeedMultiplier;

        rb.AddForce(force);

        // Apply general damping
        rb.linearVelocity *= linearDamp;

        // Extra damping if grounded
        if(isGrounded)
        {
            rb.linearVelocity *= linearDamp * linearDamp;
        }
    }

    // Manages the hand position and player attachment to climbing surfaces.
    void HandleHand()
    {
        if (!handObj.isAnchored)
        {
            // Save velocity for momentum boost when grabbing
            velocityBeforeGrab = rb.linearVelocity;

            Vector3 handPos = transform.position + camTransform.forward * handDist;
            Vector3 target = handPos - hand.transform.position;
            hand.linearVelocity = Vector3.Lerp(hand.linearVelocity, target * 10, handDamp);
        }
        else //otherwise move player
        {
            Vector3 playerPos = hand.transform.position + camTransform.forward * -handDist;
            Vector3 target = playerPos - transform.position;
            Vector3 newVelocity = Vector3.Lerp(rb.linearVelocity, target * 10, handDamp);

            // Apply momentum boost only at the beginning of grab
            if (!wasAnchored)
            {
                newVelocity += velocityBeforeGrab * momentumBoost;
            }

            rb.linearVelocity = newVelocity;
        }

        // Update previous state
        wasAnchored = handObj.isAnchored;

        float distSqr = (hand.transform.position - transform.position).sqrMagnitude;
        if (distSqr > maxHandDist * maxHandDist)
        {
            hand.transform.position = transform.position;
            hand.linearVelocity = Vector3.zero;
        }
    }

    // Manages wind and footstep audio based on player speed and grounded state.
    void HandleAudio()
    {
        if (windAudioSource == null || footstepsAudioSource == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);

        // Wind sound - plays when in air OR when jumping (large vertical velocity)
        bool shouldPlayWind = (!isGrounded || verticalSpeed > 0.5f) && currentSpeed > windVelocityThreshold;

        if (shouldPlayWind)
        {
            if (!windAudioSource.isPlaying && windSound != null)
            {
                windAudioSource.Play();
            }

            // Calculate volume based on velocity - minimum volume to be heard always
            float velocityRatio = Mathf.Clamp01((currentSpeed - windVelocityThreshold) / (maxWindVelocity - windVelocityThreshold));
            // Volume between 0.3 and maxWindVolume for better hearing
            float targetVolume = Mathf.Lerp(0.3f, maxWindVolume, velocityRatio);

            // Fade in very quickly for instant response
            windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, targetVolume, Time.deltaTime * 20f);
        }
        else
        {
            // Fade out quickly when landing
            windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, 0f, Time.deltaTime * 12f);
        }

        // Footsteps sound - only when grounded and moving horizontally (without large vertical velocity)
        bool shouldPlayFootsteps = isGrounded && horizontalSpeed > 0.05f && verticalSpeed < 0.5f;

        if (shouldPlayFootsteps)
        {
            if (!footstepsAudioSource.isPlaying && footstepsSound != null)
            {
                footstepsAudioSource.Play();
            }

            // Volume based on horizontal speed - minimum volume to be heard always
            float speedRatio = Mathf.Clamp01(horizontalSpeed / maxSpeed);
            // Volume between 0.4 and footstepVolume to be heard always while walking
            float targetFootstepVolume = Mathf.Lerp(0.4f, footstepVolume, speedRatio);
            footstepsAudioSource.volume = Mathf.Lerp(footstepsAudioSource.volume, targetFootstepVolume, Time.deltaTime * 15f);
        }
        else
        {
            // Fade out quickly when stopping or jumping
            footstepsAudioSource.volume = Mathf.Lerp(footstepsAudioSource.volume, 0f, Time.deltaTime * 18f);
        }
    }

    // Manages stamina regeneration and depletion based on climbing state.
    void HandleStamina()
    {
        if(handObj.isAnchored) {
            stamina -= Time.deltaTime * staminaDrainMult;
            if(stamina <= 0) {
                stamina = Mathf.Max(stamina, 0);
                exhausted = true;
            }
        }
        if(!isGrounded) return;
        stamina += Time.deltaTime * staminaRegenMult;
        stamina = Mathf.Min(stamina, maxStamina);
        if(stamina > 0.5f) exhausted = false;
    }

}
