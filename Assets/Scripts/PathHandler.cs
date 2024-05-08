using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHandler : MonoBehaviour
{
    public GameObject LightPath;
    public GameObject DarkPath;
    PowerSelection powerSelection;

    // Start is called before the first frame update
    void Start()
    {
        powerSelection = FindObjectOfType<PowerSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (powerSelection.ChosenDark)
        {
            DarkPath.SetActive(false);
        }

        if (powerSelection.ChosenLight) 
        {
            LightPath.SetActive(false);
        }
    }
}
