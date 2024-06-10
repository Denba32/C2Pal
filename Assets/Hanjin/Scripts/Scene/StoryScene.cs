using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryScene : BaseScene
{
    protected override void Clear()
    {
        throw new System.NotImplementedException();
    }

    protected override void Init()
    {
        base.Init();

    }

    public void GoToNextScene()
    {
        SceneManagerEx.Instance.ChangeScene(Define.SceneType.MainScene);
    }
}
