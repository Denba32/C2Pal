using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UIConditions uiconditions;
    

    Condition health { get { return uiconditions.health; } }
    Condition hunger { get { return uiconditions.hunger; } }
    Condition stamina { get { return uiconditions.stamina; } }

    public float noHungerHealthDecay;
    void Update()
    {
        hunger.Substract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if(hunger.curValue <= 0f)
        {
            health.Substract(health.passiveValue * Time.deltaTime);
        }

        if(health.curValue <= 0f)
        {
            Die();
        }
        
    }
    public bool Heal(float amount)
    {
        if(health.curValue + amount > health.maxValue)
        {
            Debug.Log("HP�� �̹� Max �����Դϴ�.");
            return false;
        }

        health.Add(amount);
        return true;
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void RestoreStamina(float amount)
    {
        stamina.Add(amount);
    }

    private void Die()
    {
        Debug.Log("����� �׾����ϴ�!");
    }
}
