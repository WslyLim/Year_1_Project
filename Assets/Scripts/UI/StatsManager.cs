using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private PlayerController player;
    private LevelSystem levelSystem;
    private LevelWindow levelWindow;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        levelSystem = FindObjectOfType<LevelSystem>();
        levelWindow = FindObjectOfType<LevelWindow>();
    }

    public void OnClick_HealthUpBtn()
    {
        if (levelSystem.GetStatPoint() > 0) 
        {
            Debug.Log("Upgraded Health");
            levelSystem.SetStatPoint(1);
            levelWindow.SetStatsTextPoint();
            player.healthSystem.AddMaxHealth(10);
        }
        
    }

    public void OnClick_AtkUpBtn() 
    {
        if (levelSystem.GetStatPoint() > 0)
        {
            Debug.Log("Upgraded Attack");
            levelSystem.SetStatPoint(1);
            levelWindow.SetStatsTextPoint();
            player.SetPlayerDamage(2);
        }
    }

    public void OnClick_DefUpBtn()
    {
        if (levelSystem.GetStatPoint() > 0)
        {
            Debug.Log("Upgraded Defense");
            levelSystem.SetStatPoint(1);
            levelWindow.SetStatsTextPoint();
            player.SetPlayerDefense(20);
        }
    }
    
}
