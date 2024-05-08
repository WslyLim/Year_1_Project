using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    public GameObject NPC1;
    public GameObject NPC2;
    public int enemiesLeft = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesLeft <= 0)
        {
            NPC1.SetActive(false);
            NPC2.SetActive(true);
        }
    }
}
