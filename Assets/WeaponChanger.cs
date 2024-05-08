using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    public GameObject FirstWeaponToDestroy;
    public GameObject SecondWeaponToDestroy;
    public GameObject WeaponToChange;
    public GameObject theEnemy;

    PlayerController player;
    public float buffDamage;
    bool claimable;
    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (theEnemy.IsDestroyed())
        {
            if (Input.GetKeyDown(KeyCode.F) && claimable)
            {
                Destroy(FirstWeaponToDestroy);
                Destroy(SecondWeaponToDestroy);
                WeaponToChange.SetActive(true);
                player.ChangeWeapon(WeaponToChange);
                player.SetPlayerDamage(buffDamage);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        claimable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        claimable = false;
    }
}
