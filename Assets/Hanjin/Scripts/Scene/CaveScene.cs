using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveScene : BaseScene
{
    public Transform respawnPoint;

    protected override void Clear()
    {
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.Castle;
        ResourceManager.Instance.Instantiate("Player/Player").transform.position = respawnPoint.position;
    }
}
