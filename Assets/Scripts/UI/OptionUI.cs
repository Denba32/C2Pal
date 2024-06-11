using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : PopupUI
{
    public Slider bgmSlider;
    public Slider effectSlider;
    
    public override void ClosePopUI()
    {
        base.ClosePopUI();

        if(Time.timeScale <= 0)
        {
            Time.timeScale = 1;
        }
    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Init()
    {
        base.Init();

        bgmSlider.value = SoundManager.Instance.bgmVolume;
        effectSlider.value = SoundManager.Instance.effectVolume;

        bgmSlider.onValueChanged.AddListener(delegate { ChangeBgmVolume(); });
        effectSlider.onValueChanged.AddListener(delegate { ChangeEffectVolume(); });

    }

    private void ChangeEffectVolume()
    {
        Debug.Log("효과음 변경");
        SoundManager.Instance.effectVolume = effectSlider.value;

    }

    private void ChangeBgmVolume()
    {
        Debug.Log("브금 변경");
        SoundManager.Instance.bgmVolume = bgmSlider.value;
    }
}