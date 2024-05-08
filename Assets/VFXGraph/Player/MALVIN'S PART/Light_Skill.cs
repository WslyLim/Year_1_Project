using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Rendering;

public class Light_Skill : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.E;
    public float throwForce;

    bool readyToThrow;
    

    private void Start()
    {
        readyToThrow = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow == true)
        {
            Throw();
        }
    }


    private void Throw()
    {
        readyToThrow = false;

        //instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        //get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();


        //add force
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        //calculate direction
        Vector3 forcedirection = cam.transform.forward;

        //cooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

}
