using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScene : BaseScene
{
    public Transform respawnPoint;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.BossStage;
        ResourceManager.Instance.Instantiate("Player/Player").transform.position = respawnPoint.position;

    }
    protected override void Clear()
    {

    }
}
