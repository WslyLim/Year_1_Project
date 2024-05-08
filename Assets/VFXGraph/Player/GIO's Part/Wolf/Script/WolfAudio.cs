using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip audioClip;
    private float vol;

    public float fireRate = 0.1f; //10 sec cooldown
    private float timeToFire;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vol = audioSource.volume;
    }

    void Update()
    {
        if (Input.GetKey("q") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0.3f;
                PlayAudio(audioClip);

                StartCoroutine(Volume());
                
            }
        }
    }

    IEnumerator Volume()
    {
        yield return new WaitForSeconds(2.0f);
        audioSource.volume = vol;
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
}
