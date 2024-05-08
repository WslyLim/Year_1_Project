using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject returnPath;
    public GameObject nextPath;
    public int enemiesLeft;

    // Start is called before the first frame update
    void Start()
    {
        enemiesLeft = Enemies.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesLeft <= 0) 
        {
            returnPath.SetActive(false);
            nextPath.SetActive(false);
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            returnPath.SetActive(true);
            nextPath.SetActive(true);
        }   
    }
}
