using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject text = null;
    [SerializeField]
    private PlayerController player;

    

    private Transform mainCam;


    // Start is called before the first frame update
    void Start()
    {
        text.gameObject.SetActive(false);
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        text.transform.rotation = Quaternion.LookRotation(text.transform.position - mainCam.transform.position);
    }

    public void PickedUp()
    {
        //player.UnlockedWeapon(gameObject);
        Debug.Log("Called Function UnlockedWeapon");
        Destroy(gameObject);

        if (gameObject.tag == "HealPotion")
        {
            player.GetComponent<HealthSystem>().Heal(20);
            Debug.Log("Heal!");
            Debug.Log(player.GetComponent<HealthSystem>().GetHealth());

        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        if (Input.GetKey(KeyCode.E))
    //        {
    //            PickedUp();
    //        }
    //    }
    //}
}
