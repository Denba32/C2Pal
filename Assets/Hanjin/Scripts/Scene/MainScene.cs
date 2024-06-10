using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene
{
    public Transform cave;
    public Transform bloodCastle;
    public Transform firstRespawn;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        SceneManagerEx.Instance.CurrentScene = Define.SceneType.MainScene;

        if (SceneManagerEx.Instance.PreviousScene == Define.SceneType.None || SceneManagerEx.Instance.PreviousScene == Define.SceneType.StartScene)
        {
            ResourceManager.Instance.Instantiate("Player/Player").transform.position = firstRespawn.position;
        }
        else if (SceneManagerEx.Instance.PreviousScene == Define.SceneType.CaveScene)
        {
            ResourceManager.Instance.Instantiate("Player/Player").transform.position = cave.position;
        }
        else if(SceneManagerEx.Instance.PreviousScene == Define.SceneType.Castle)
        {
            ResourceManager.Instance.Instantiate("Player/Player").transform.position = bloodCastle.position;

        }
    }

    protected override void Clear()
    {
        
    }
}