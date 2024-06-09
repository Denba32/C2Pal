using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.Start;
    }

    protected override void Clear()
    {

    }
}
