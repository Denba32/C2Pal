using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutScenePlayer : MonoBehaviour
{
    public PlayableAsset bossScene;
    public GameObject boss;
    public GameObject outGuard;
    public PlayableDirector director;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayCutScene();
        }
    }

    public void PlayCutScene()
    {
        if(director != null)
        {
            if(bossScene != null)
            {
                GameManager.Instance.GamePause();
                UIManager.Instance.ShowMainSceneUI(false);
                director.Play(bossScene);

            }
        }
    }

    public void StartGame()
    {
        UIManager.Instance.ShowMainSceneUI(true);
        GameManager.Instance.GameStart();
    }

    public void AppearBoss()
    {
        boss.SetActive(true);
        director.Stop();
        outGuard.SetActive(true);
    }
}
