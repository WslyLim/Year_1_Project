using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    [SerializeField] private Vector3 vectorPoint;
    private List<GameObject> checkpoints;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            vectorPoint = other.transform.position;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public Vector3 GetCheckPoint()
    {
        return vectorPoint;
    }
}
