using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private PlayerSkills playerSkills;
    public GameObject darkPower;
    public GameObject lightPower;
    [SerializeField] private GameObject skillType;
    [SerializeField] private GameObject skillTreePanel;
    private LevelSystem playerLevel;

    private void Start()
    {
        playerLevel = FindObjectOfType<LevelSystem>();
        skillTreePanel.SetActive(false);
        darkPower.SetActive(false);
        lightPower.SetActive(false);
        SetPlayerSkills(player.GetPlayerSkill());
    }

    public void SetPlayerSkills(PlayerSkills playerSkills)
    {
        this.playerSkills = playerSkills;
    }

    public void IsDarkPower()
    {
        Debug.Log("You Have Chosen DarkPower");
        skillType = darkPower;
        skillType.SetActive(true);

    }

    public void IsLightPower()
    {
        Debug.Log("You Have Chosen Light Power");
        skillType = lightPower;
        skillType.SetActive(true);
    }


    public void OnClickOpenSkillTree()
    {
        skillTreePanel.SetActive(true);
    }

    public void OnClickBackButton()
    {
        skillTreePanel.SetActive(false);
    }






    //------------- Dark Skills --------------//

    public void OnClick_UnlockSummonBeastBtn()
    {
        if (playerLevel.GetLevel() >= 3)
            playerSkills.TryUnlockSkill(PlayerSkills.SkillType.SummonWolf);
    }

    public void OnClick_UnlockDarkBeamBtn()
    {
        Debug.Log("Unlocking Dark Beam");

        if (playerLevel.GetLevel() >= 10)
            playerSkills.TryUnlockSkill(PlayerSkills.SkillType.DarkBeam);
    }

    //------------- Light Skills --------------//
    public void OnClick_UnlockLightOrbBtn()
    {
        if (playerLevel.GetLevel() >= 3)
            playerSkills.TryUnlockSkill(PlayerSkills.SkillType.LightOrb);
        else
            Debug.Log("Level Too Low");
    }

    public void OnClick_UnlockHolyBeamBtn()
    {
        Debug.Log("Unlocking Holy Beam");

        if (playerLevel.GetLevel() >= 10)
            playerSkills.TryUnlockSkill(PlayerSkills.SkillType.HolyBeam);
        else
            Debug.Log("Level Too Low");

    }

}
