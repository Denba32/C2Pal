using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : BaseUI 
{
    public int sortOrder;
    public override void Init()
    {
        UIManager.Instance.SetCanvas(gameObject, true);
    }
    public virtual void ClosePopUI()
    {
        UIManager.Instance.ClosePopupUI(this);
    }
}