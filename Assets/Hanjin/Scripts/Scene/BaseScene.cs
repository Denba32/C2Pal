using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType { get; protected set; } = Define.SceneType.Main;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init() { }

    protected abstract void Clear();

    public virtual void Enter() { }
    public virtual void Exit()
    {
        Clear();
    }

}
