using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth;
    private bool canHeal;

    public event EventHandler OnHealthChanged;

    public void Damage(float amount)
    { 
        health -= amount;

        if (health <= 0) 
        {
            //dead
            canHeal = false;
            health = 0;
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);

    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)health / maxHealth;
    }

    public void SetHealth(float health) 
    {
        this.health = health;
    }

    public void SetMaxHealth(float maxHealth)
    { 
        this.maxHealth = maxHealth;
    }

    public void AddMaxHealth(float value)
    {
        maxHealth += value;
    }

    public void Heal (float amount) 
    {
        health += amount;
        if (health > maxHealth && canHeal) 
        {
            health = maxHealth;
            if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
        }
    }
}
