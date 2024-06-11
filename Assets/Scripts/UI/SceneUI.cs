using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUI : BaseUI
{
    public override void Init()
    {
        UIManager.Instance.SetCanvas(gameObject, false);
    }
}
