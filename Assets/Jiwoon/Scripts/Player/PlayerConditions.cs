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

    private void Die()
    {
        Debug.Log("당신은 죽었습니다!");
    }
}
