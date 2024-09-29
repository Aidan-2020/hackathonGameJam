using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Detection")]
    public float groundDrag = 6f;
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        walking,
        air,
        sliding
    }

    public bool freeze;
    public bool activeGrapple;
    public bool sliding;
    public bool hasSlideJumped;

    public GameObject stopWatch;
    bool stratedLevel = false;

    [SerializeField] private InputActionAsset playerControls;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    public bool isMovingForward;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle drag
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        // Turn off gravity while on slope
        rb.useGravity = !OnSlope();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        // When to jump
        if (jumpAction.triggered && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        //Mode - freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
        }
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple)
        {
            return;
        }

        // Calculate movement direction
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        // Check if player is moving forward
        isMovingForward = Vector3.Dot(moveDirection.normalized, orientation.forward) > 0.7f;

        // On slope
        if (OnSlope() && !hasSlideJumped)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * movementMultiplier, ForceMode.Force);
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // On ground
        else if (grounded)
        {
            if (moveSpeed == 0) // fix movespeed being stuck at zero when it should not be
            {
                moveSpeed = 10;
            }
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Force);
        }
        // In air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Force);
    }
    private void SpeedControl()
    {
        if (activeGrapple)
        {
            return;
        }

        // Limiting speed on slope
        if (OnSlope() && !hasSlideJumped)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        // Limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        hasSlideJumped = false;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void resetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (stratedLevel == false)
        {
            stopWatch.GetComponent<StopWatch>().isRunning = true;
            stratedLevel = true;
        }
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            resetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}

