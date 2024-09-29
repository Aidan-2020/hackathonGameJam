using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overShootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [SerializeField] private InputActionAsset playerControls;

    private InputAction grappleAction;

    private bool grappling;

    private void Awake()
    {
        grappleAction = playerControls.FindActionMap("Player").FindAction("Grapple");

        pm = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        grappleAction.Enable();
    }

    private void OnDisable()
    {
        grappleAction.Disable();
    }

    private void Update()
    {
        if (grappleAction.triggered)
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }
    private void StartGrapple()
    {
        if(grapplingCdTimer > 0)
        {
            return;
        }

        grappling = true;
        pm.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if(grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overShootYAxis;
        }

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

}
