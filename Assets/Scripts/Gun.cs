using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

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
