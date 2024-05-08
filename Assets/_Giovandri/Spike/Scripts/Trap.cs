using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Trap : MonoBehaviour
{
    PlayerController player;
    public float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        while (true)
        {
            GetComponent<Animation>().Play();
            yield return new WaitForSeconds(2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.healthSystem.Damage(player.healthSystem.GetMaxHealth());
        }
    }
}
