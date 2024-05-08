using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_GameHandler : MonoBehaviour
{
    public HealthBar healthBar;
    //HealthSystem healthSystem = new HealthSystem(100);
    private HealthSystem healthSystem;

    // Start is called before the first frame update
    void Start()
    {
        healthSystem = gameObject.GetComponent<HealthSystem>();
        healthBar.Setup(healthSystem);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
