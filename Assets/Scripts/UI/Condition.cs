using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float startValue;
    public float passiveValue;
    public Image uiBar;
    public TextMeshProUGUI text;
    
    void Start()
    {
        curValue = startValue;
    }

    
    void Update()
    {
        uiBar.fillAmount = Getpercentage();
        GetText();
    }

    float Getpercentage()
    {
        return curValue / maxValue; ;
    }
    public void GetText()
    {
        text.text = $"{curValue.ToString("N0")}/{maxValue.ToString("N0")}";
    }
    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Substract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }
}
