using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    private DataManager _data;
    public DataManager Data
    {
        get
        {
            if(_data == null )
            {
                _data = new DataManager();

            }
            return _data;
        }
    }
    public event Action onGameStart;
    public event Action onGamePause;
    private void Start()
    {
        Data.Init();
    }
    public void GameStart()
    {
        onGameStart?.Invoke();
    }
    public void GamePause()
    {
        onGamePause?.Invoke();
    }
}