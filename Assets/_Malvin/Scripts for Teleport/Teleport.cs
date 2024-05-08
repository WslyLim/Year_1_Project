using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform TeleportDestination;
    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Must disable first to teleport properly
            player.controller.enabled = false; 

            // Teleport to destination (Final level boss area)
            player.controller.transform.position = TeleportDestination.transform.position;
            Debug.Log("Teleported");

            // Enable again to move
            player.controller.enabled = true;
        }

    }
}
