using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetState : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    public AudioClip patrolAudioClip;
    public AudioClip attackAudioClip;

    public Transform playerTransform;
    public float maxHearingDistance = 15f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Check the current state and perform actions accordingly
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Patrol State"))
        {
            // Calculate distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            // Adjust audio volume based on distance
            AdjustVolumeBasedOnDistance(distanceToPlayer);

            if (!audioSource.isPlaying)
            {
                PlayAudio(patrolAudioClip);
            }
        }
        else if (stateInfo.IsName("Chase State"))
        {
            // Calculate distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            // Adjust audio volume based on distance
            AdjustVolumeBasedOnDistance(distanceToPlayer);

            if (!audioSource.isPlaying)
            {
                PlayAudio(patrolAudioClip);
            }
        }
        else if (stateInfo.IsName("Attack State"))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                PlayAudio(attackAudioClip);
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Audio clip not assigned or AudioSource not found!");
        }
    }

    private void AdjustVolumeBasedOnDistance(float distance)
    {
        // Calculate a normalized volume based on distance
        float adjustedVol = 1f - (distance / maxHearingDistance);

        // Adjust the audio source volume
        if (audioSource != null)
        {
            audioSource.volume = adjustedVol;
        }
    }
}
