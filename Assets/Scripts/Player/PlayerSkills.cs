using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkills : MonoBehaviour
{
    int currentLevel;
    private LevelWindow _levelWindow;
    [SerializeField] private LevelSystem _levelSystem;
    private List<SkillType> unlockedSkillTypeList;

    public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;
    public class OnSkillUnlockedEventArgs : EventArgs
    {
        public SkillType skillType;
    }

    void Start()
    {
        _levelWindow = FindObjectOfType<LevelWindow>();
        _levelSystem = GetComponent<LevelSystem>();
    }

    public enum SkillType
    {
        None,

        // DARK TYPE SKILLS //
        SummonWolf,
        DarkBeam,

        // LIGHT TYPE SKILLS //
        LightOrb,
        HolyBeam,

    }

    public PlayerSkills()
    {
        unlockedSkillTypeList = new List<SkillType>();
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (!IsSkillUnlocked(skillType))
        {
            unlockedSkillTypeList.Add(skillType);
            //OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { skillType = skillType });
        }
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }

    public SkillType GetSkillRequirement(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.DarkBeam: return SkillType.SummonWolf;

            case SkillType.HolyBeam: return SkillType.LightOrb;
        }
        return SkillType.None;
    }

    public int GetLevelRequirement(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.SummonWolf:
                return 3;


            case SkillType.DarkBeam:
                return 10;


            case SkillType.LightOrb:
                return 3;


            case SkillType.HolyBeam:
                return 10;

            default:
                return 0;
        }

    }

    public bool TryUnlockSkill(SkillType skillType)
    {
        SkillType skillRequirement = GetSkillRequirement(skillType);

        //int requiredLevel = GetLevelRequirement(skillType);
        //Debug.Log(requiredLevel);

        if (skillRequirement != SkillType.None)
        {
            if (IsSkillUnlocked(skillRequirement))
            {
                UnlockSkill(skillType);


                Debug.Log("Unlocked " + skillType);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            UnlockSkill(skillType);
            Debug.Log("Unlocked " + skillType);
            return true;
        }


    }

}
