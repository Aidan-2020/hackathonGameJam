using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    public GameObject stopWatch;

    bool stratedLevel = false;

    //public InputAction playerControls;

    //[Header("Keybinds")]
    //public KeyCode jumpKey = Input.GetButton("Jump");

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    //unity new control scheme
    [SerializeField] private InputActionAsset playerControls;

    private InputAction moveAction;
    //private InputAction lookAction;
    private InputAction jumpAction;
    //private InputAction shootAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopHit;
    private bool exitingSlope;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        //lookAction = playerControls.FindActionMap("Player").FindAction("Look");
        jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");
        //shootAction = playerControls.FindActionMap("Player").FindAction("Shoot");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
      //  lookAction.Enable();
        jumpAction.Enable();
        //shootAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
       // lookAction.Disable();
        jumpAction.Disable();
        //shootAction.Disable();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        //turn off gravity while on slope
        rb.useGravity = !OnSlope();
    }

    private void MyInput()
    {
       // horizontalInput = Input.GetAxisRaw("Horizontal");
        //verticalInput = Input.GetAxisRaw("Vertical");


        //when to jump
        if(jumpAction.triggered && readyToJump && grounded)
        {
            //print("work");
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        // moveDirection = playerControls.ReadValue<Vector3>();
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {

        //limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (stratedLevel == false)
        {
            stopWatch.GetComponent<StopWatch>().isRunning = true;
            stratedLevel = true;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;

    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopHit.normal).normalized;
    }
}
