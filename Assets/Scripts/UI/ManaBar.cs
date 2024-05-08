using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ManaBar : MonoBehaviour
{
    public Image frontManaBar;
    public Image backManaBar;
    private ManaSystem manaSystem;

    private float lerpTimer;
    public float chipSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateManaGUI();
    }

    public void Setup(ManaSystem manaSystem)
    {
        this.manaSystem = manaSystem;
        manaSystem.OnManaChanged += ManaSystem_OnHealthChanged;
    }

    private void ManaSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        lerpTimer = 0f;
    }
    public void UpdateManaGUI()
    {
        float fillF = frontManaBar.fillAmount;
        float fillB = backManaBar.fillAmount;
        float hFraction = manaSystem.GetManaPercentage();

        if (fillB > hFraction)
        {
            frontManaBar.fillAmount = hFraction;
            backManaBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backManaBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        if (fillF < hFraction)
        {
            backManaBar.fillAmount = hFraction;
            backManaBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontManaBar.fillAmount = Mathf.Lerp(fillF, backManaBar.fillAmount, percentComplete);

        }
    }
}
