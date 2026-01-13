using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    public float momentumBoost = 0.5f; // cat din viteza de alergare se adauga la swing
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
        windAudioSource.spatialBlend = 0f; // 2D sound

        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        footstepsAudioSource.clip = footstepsSound;
        footstepsAudioSource.loop = true;
        footstepsAudioSource.volume = 0f;
        footstepsAudioSource.playOnAwake = false;
        footstepsAudioSource.spatialBlend = 0f; // 2D sound

        // Verifică dacă clipurile audio sunt setate
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

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleHand();
        HandleStamina(); // call after hand for Anchored status
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        Vector3 spherePosition = transform.position;
        
        // Create layer mask that ignores Player layer
        int layerMask = ~LayerMask.GetMask("Player");
        
        // Check with raycast first
        bool rayHit = Physics.Raycast(spherePosition, Vector3.down, out hit, groundCheckDistance, layerMask);
        // Also check with sphere cast for better detection
        bool sphereHit = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, layerMask);
        isGrounded = rayHit || sphereHit;
        
        // Additional check
        if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.5f)
        {
            isGrounded = Physics.Raycast(spherePosition, Vector3.down, groundCheckDistance * 1.5f, layerMask);
        }
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

    void HandleHand()
    {
        if (!handObj.isAnchored)
        {
            // Salveaza viteza pentru momentum boost cand face grab
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

            // Aplica momentum boost doar la inceputul grab-ului
            if (!wasAnchored)
            {
                newVelocity += velocityBeforeGrab * momentumBoost;
            }

            rb.linearVelocity = newVelocity;
        }

        // Update starea anterioara
        wasAnchored = handObj.isAnchored;

        float dist = Vector3.Distance(hand.transform.position, transform.position);
        if (dist > maxHandDist)
        {
            hand.transform.position = transform.position;
            hand.linearVelocity = Vector3.zero;
        }
    }

    void HandleAudio()
    {
        if (windAudioSource == null || footstepsAudioSource == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);

        // Wind sound - se aude când ești în aer SAU când ai viteză verticală mare (săritura)
        bool shouldPlayWind = (!isGrounded || verticalSpeed > 0.5f) && currentSpeed > windVelocityThreshold;

        if (shouldPlayWind)
        {
            if (!windAudioSource.isPlaying && windSound != null)
            {
                windAudioSource.Play();
            }

            // Calculează volumul bazat pe viteză - volum minim mai mare pentru a se auzi mereu
            float velocityRatio = Mathf.Clamp01((currentSpeed - windVelocityThreshold) / (maxWindVelocity - windVelocityThreshold));
            // Volum între 0.3 și maxWindVolume pentru a se auzi mai bine
            float targetVolume = Mathf.Lerp(0.3f, maxWindVolume, velocityRatio);

            // Fade in foarte rapid pentru răspuns instant
            windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, targetVolume, Time.deltaTime * 20f);
        }
        else
        {
            // Fade out rapid când aterizezi
            windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, 0f, Time.deltaTime * 12f);
        }

        // Footsteps sound - doar când ești pe pământ și te miști orizontal (fără viteză verticală mare)
        bool shouldPlayFootsteps = isGrounded && horizontalSpeed > 0.05f && verticalSpeed < 0.5f;

        if (shouldPlayFootsteps)
        {
            if (!footstepsAudioSource.isPlaying && footstepsSound != null)
            {
                footstepsAudioSource.Play();
            }

            // Volume based on horizontal speed - volum minim mai mare
            float speedRatio = Mathf.Clamp01(horizontalSpeed / maxSpeed);
            // Volum între 0.4 și footstepVolume pentru a se auzi mereu când mergi
            float targetFootstepVolume = Mathf.Lerp(0.4f, footstepVolume, speedRatio);
            footstepsAudioSource.volume = Mathf.Lerp(footstepsAudioSource.volume, targetFootstepVolume, Time.deltaTime * 15f);
        }
        else
        {
            // Fade out rapid când te oprești sau sari
            footstepsAudioSource.volume = Mathf.Lerp(footstepsAudioSource.volume, 0f, Time.deltaTime * 18f);
        }
    }

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
