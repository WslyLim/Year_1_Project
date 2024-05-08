using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSelection : MonoBehaviour
{
    [Header("Skill Tree")]

    public EventHandler OnDarkPowerChosen;
    public EventHandler OnLightPowerChosen;

    public bool ChosenLight = false;
    public bool ChosenDark = false;
    private SkillTree skillTree;
    private Transform PowerSelectionPanel;
    private Hovl_DemoLasers laserType;
    // Start is called before the first frame update
    void Start()
    {
        skillTree = GetComponent<SkillTree>();
        PowerSelectionPanel = transform.Find("PowerSelectionPanel");
        PowerSelectionPanel.gameObject.SetActive(true);
        laserType = FindObjectOfType<Hovl_DemoLasers>();
    }

    public void DarkPowerChosen()
    {
        skillTree.IsDarkPower();
        PowerSelectionPanel.gameObject.SetActive(false);
        laserType.PlayerPowerType(true);
        ChosenDark = true;
    }

    public void LightPowerChosen() 
    {
        skillTree.IsLightPower();
        PowerSelectionPanel.gameObject.SetActive(false);
        laserType.PlayerPowerType(false);
        ChosenLight = true;
    }


}
