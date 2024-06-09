using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainSceneUI : SceneUI
{
    public UIConditions uiConditions;

    public TMP_Text alertText;

    private Coroutine coAlert;
    public override void Init()
    {
        base.Init();

        alertText.gameObject.SetActive(false);
    }


    public void SetAlert(string message)
    {
        if(coAlert != null)
            StopCoroutine(coAlert);

        coAlert = StartCoroutine(SetAlertText(message));
    }

    private IEnumerator SetAlertText(string message)
    {
        alertText.text = "";
        alertText.gameObject.SetActive(true);
        alertText.text = message;
        yield return CoroutineHelper.WaitForSeconds(3f);
        alertText.gameObject.SetActive(false);
        yield break;
    }
}