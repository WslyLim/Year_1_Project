using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    [SerializeField]
    private float mana;
    [SerializeField]
    private float maxMana;

    public event EventHandler OnManaChanged;

    public void Consume(float amount)
    {
        mana -= amount;
        if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
    }

    public float GetMana()
    {
        return mana;
    }

    public float GetManaPercentage()
    {
        return (float)mana / maxMana;
    }

    public void RecoverMana(float amount)
    {
        mana += amount;
        if (mana > maxMana)
        {
            mana = maxMana;
            if (OnManaChanged != null) OnManaChanged(this, EventArgs.Empty);
        }
    }

}
