using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{

    private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statsPointText;
    [SerializeField] private TextMeshProUGUI showHealthText;
    [SerializeField] private TextMeshProUGUI showAttackText;
    [SerializeField] private TextMeshProUGUI showDefenseText;
    [SerializeField] private Image ExpBar;

    private LevelSystem levelSystem;
    private PlayerController player;

    private float lerpTimer = 0;
    private float chipSpeed = 2f;

    private void Awake()
    {
        levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        ExpBar = transform.Find("ExpBar").GetComponent<Image>();
        player = FindObjectOfType<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetStatsTextPoint();
    }

    private void SetExperienceBarSize(float experienceNormalized)
    {
        ExpBar.fillAmount = experienceNormalized;
    }


    private void SetLevelNumber(int levelNumber)
    {
        levelText.text = "Level\n" + (levelNumber);
    }

    public void SetStatsTextPoint()
    {
        statsPointText.text = "Your Stats Point " + levelSystem.GetStatPoint();
        showHealthText.text = "Your HP: " + player.healthSystem.GetHealth() + "/" + player.healthSystem.GetMaxHealth();
        showAttackText.text = "Your Attack: " + player.GetPlayerDamage();
        showDefenseText.text = "Your Defense: " + player.GetPlayerDefense();
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        
        this.levelSystem = levelSystem;

        SetLevelNumber(levelSystem.GetLevel());
        SetExperienceBarSize(levelSystem.GetExperienceNormalized());

        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
        levelSystem.OnStatsPointChanged += LevelSystem_OnStatsPointChanged;
        
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        lerpTimer = 0;
        SetLevelNumber(levelSystem.GetLevel());

        
    }
    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        lerpTimer = 0;
        SetExperienceBarSize(levelSystem.GetExperienceNormalized());
        
    }

    private void LevelSystem_OnStatsPointChanged(object sender, System.EventArgs e)
    {
        levelSystem.AddStatPoint();
        SetStatsTextPoint();
    }



    
}
