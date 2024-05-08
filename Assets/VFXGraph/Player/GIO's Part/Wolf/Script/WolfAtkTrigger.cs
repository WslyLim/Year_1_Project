using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAtkTrigger : MonoBehaviour
{
    public Camera cam;
    public GameObject projectile;
    public Transform firePoint;
    public float fireRate = 0.1f; //10 sec cooldown

    private Vector3 destination;
    private float timeToFire;
    private WolfAtk wolfAtkScript;
    PlayerController player;


    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKey("q") && Time.time >= timeToFire)
        {
            player.manaSystem.Consume(30);
            timeToFire = Time.time + 1 / fireRate;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (cam != null)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            destination = ray.GetPoint(1000);
            InstantiateProjectile();
        }
        else
        {
            Debug.Log("B");
            InstantiateProjectileAtFirePoint();
        }
    }

    void InstantiateProjectile()
    {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;

        wolfAtkScript = projectileObj.GetComponent<WolfAtk>();
        RotateToDestination(projectileObj, destination, true);
        projectileObj.GetComponent<Rigidbody>().velocity = transform.forward * wolfAtkScript.speed;
    }

    void InstantiateProjectileAtFirePoint()
    {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;

        wolfAtkScript = projectileObj.GetComponent<WolfAtk>();
        RotateToDestination(projectileObj, firePoint.transform.forward * 1000, true);
        projectileObj.GetComponent<Rigidbody>().velocity = firePoint.transform.forward * wolfAtkScript.speed;
    }

    void RotateToDestination(GameObject obj, Vector3 destination, bool onlyY)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if (onlyY)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    
}
