using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveScene : BaseScene
{
    public override void Enter()
    {
        base.Enter();
        SceneType = Define.SceneType.Castle;
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Clear()
    {
    }

    protected override void Init()
    {
        base.Init();
    }
}
