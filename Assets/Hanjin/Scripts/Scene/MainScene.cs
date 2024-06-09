using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.Main;
    }

    protected override void Clear()
    {
        
    }
}