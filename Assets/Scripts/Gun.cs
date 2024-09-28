using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    //stats
    public float range, timeBetweenShots;

    public int bulletsPerTap;
    public bool allowButtonHold;

    //bools
    bool shooting, reloading;
    bool readyToShoot = true;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsPlant;


    [SerializeField] private InputActionAsset playerControls;

    private InputAction shootAction;

    private void Awake()
    {
        shootAction = playerControls.FindActionMap("Player").FindAction("Shoot");
    }

    private void Update()
    {
        MyInput();
    }

    private void OnEnable()
    {
        shootAction.Enable();
    }

    private void OnDisable()
    {
        shootAction.Disable();
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = shootAction.triggered;
        else shooting = shootAction.triggered;

        //Shoot
        if(readyToShoot && shooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out rayHit, range, whatIsPlant))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Plant"))
            {
                //get the plant script and make it grow
                //rayHit.collider.GetComponent
            }

        }

        Invoke("ResetShot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
}
