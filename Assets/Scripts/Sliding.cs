using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime = 1f;
    public float slideForce = 200f;
    public float slideYScale = 0.5f;
    private float slideTimer;
    private float startYScale;

    [Header("Slide Jumping")]
    public float slideJumpUpForce = 8f;
    public float slideJumpForwardForce = 10f;
    public float maxSlideJumpTime = 0.5f;
    private float slideJumpTimer;

    [SerializeField] private InputActionAsset playerControls;

    private InputAction slideAction;
    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveInput;

    private void Awake()
    {
        slideAction = playerControls.FindActionMap("Player").FindAction("Slide");
        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        slideAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        slideAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        if (slideAction.IsPressed() && (moveInput.x != 0 || moveInput.y != 0) && !pm.sliding && pm.OnSlope())
        {
            StartSlide();
        }

        if (!slideAction.IsPressed() && pm.sliding)
        {
            StopSlide();
        }

        if (pm.sliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0 || !pm.OnSlope())
            {
                StopSlide();
            }
        }

        if (!pm.sliding && slideJumpTimer > 0)
        {
            slideJumpTimer -= Time.deltaTime;
            if (jumpAction.WasPressedThisFrame())
            {
                SlideJump();
            }
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
    }

    private void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        slideJumpTimer = maxSlideJumpTime;
    }

    private void SlideJump()
    {
        StopSlide();

        Vector3 jumpDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        jumpDirection = jumpDirection.normalized;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * slideJumpUpForce, ForceMode.Impulse);
        rb.AddForce(jumpDirection * slideJumpForwardForce, ForceMode.Impulse);

        slideJumpTimer = 0f;
        pm.hasSlideJumped = true;
    }
}
