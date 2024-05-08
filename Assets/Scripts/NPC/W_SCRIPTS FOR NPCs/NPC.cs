using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject path;
    public event EventHandler OnInteract;
    public int reward;
    public int exp;
    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void Interact()
    {
        // Start Dialogue Interaction
        OnInteract?.Invoke(this, EventArgs.Empty);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        // Unlock the path
        path.SetActive(false);

        // Give player reward
        // If the npc doesn't give any reward, set "reward" and "exp" to 0
        player.ReceiveReward(reward);
        player.levelSystem.AddExperience(exp);
    }

}
