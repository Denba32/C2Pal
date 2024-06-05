using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action onGameStart;
    public event Action onGamePause;

    public void GameStart()
    {
        onGameStart?.Invoke();
    }
    public void GamePause()
    {
        onGamePause?.Invoke();
    }
}
