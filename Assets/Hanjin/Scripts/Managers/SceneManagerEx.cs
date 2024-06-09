using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : Singleton<SceneManagerEx>
{
    public BaseScene CurrentScene { get; private set; }

    public Coroutine coSceneChanger;

    AsyncOperation asyncLoad;
    public void ChangeScene(BaseScene nextScene)
    {
        CurrentScene.Exit();
        LoadScene(nextScene);

    }

    public void LoadScene(BaseScene nextScene)
    {
        if( coSceneChanger != null )
        {
            StopCoroutine(coSceneChanger);
        }

        coSceneChanger = StartCoroutine(LoadAsyncScene(nextScene.SceneType));
    }

    private IEnumerator LoadAsyncScene(Define.SceneType type)
    {
        // TODO �ε�ȭ�� Ű��


        asyncLoad = SceneManager.LoadSceneAsync(GetSceneName(type));

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // TODO �ε�ȭ�� ����

        asyncLoad.allowSceneActivation = true;
    }

    private string GetSceneName(Define.SceneType type)
    {
        return Enum.GetName(typeof(Define.SceneType), type);
    }
}