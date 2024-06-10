using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : Singleton<SceneManagerEx>
{
    public Define.SceneType CurrentScene  = Define.SceneType.MainScene;

    public Define.SceneType PreviousScene = Define.SceneType.None;

    public Coroutine coSceneChanger;

    public LoadingUI Loading;

    AsyncOperation asyncLoad;
    public void ChangeScene(Define.SceneType nextScene)
    {
        if (coSceneChanger != null)
        {
            StopCoroutine(coSceneChanger);
        }
        PreviousScene = CurrentScene;
        CurrentScene = nextScene;

        coSceneChanger = StartCoroutine(LoadAsyncScene(nextScene));
    }

    private IEnumerator LoadAsyncScene(Define.SceneType type)
    {

        // TODO 로딩화면 키기
        if(Loading == null)
        {
            Loading = UIManager.Instance.ShowPopupUI<LoadingUI>();
        }
        UIManager.Instance.ShowMainSceneUI(false);
        Loading.gameObject.SetActive(true);
        asyncLoad = SceneManager.LoadSceneAsync(GetSceneName(type));

        asyncLoad.allowSceneActivation = false;


        Clear();

        while (asyncLoad.progress < 0.9f)
        {
            Loading.slider_Progress.value = asyncLoad.progress;
            yield return null;
        }
        yield return CoroutineHelper.WaitForSeconds(1f);


        asyncLoad.allowSceneActivation = true;
        // TODO 로딩화면 끄기
        Loading.gameObject.SetActive(false);
        
    }

    private string GetSceneName(Define.SceneType type)
    {
        return Enum.GetName(typeof(Define.SceneType), type);
    }

    private void Clear()
    {
        GameManager.Instance.Data.Clear();
        UIManager.Instance.Clear();
        CharacterManager.Instance.Player.inventory.PlayerInven.Clear();
    }
}