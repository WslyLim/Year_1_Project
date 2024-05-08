using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class LevelSystem : MonoBehaviour
{
    private int level;
    private int experience;
    private int experienceToNextLevel;
    private int statsPoint;

    public EventHandler OnExperienceChanged;
    public EventHandler OnLevelChanged;
    public EventHandler OnStatsPointChanged;
    // Start is called before the first frame update
    void Start()
    {

        level = 0;
        experience = 0;
        experienceToNextLevel = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickAddExperience()
    {
        AddExperience(500);
    }

    public void AddStatPoint()
    {
        statsPoint++;
    }

    public int GetStatPoint()
    {
        return statsPoint;
    }

    public void SetStatPoint(int value)
    {
        statsPoint -= value;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            level++;
            experience -= experienceToNextLevel;
            IncreaseExpToNextLevel();

            OnLevelChanged?.Invoke(this, EventArgs.Empty);
            OnStatsPointChanged?.Invoke(this, EventArgs.Empty);
        }
        OnExperienceChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetExperienceNormalized()
    {
        return (float)experience / experienceToNextLevel;
    }

    public void IncreaseExpToNextLevel()
    {
        experienceToNextLevel += 100;
    }
}
