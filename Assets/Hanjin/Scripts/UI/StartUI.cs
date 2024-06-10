
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Button startButton;

    public Button quitButton;


    private void OnEnable()
    {
        startButton.onClick.AddListener(GameStart);
        quitButton.onClick.AddListener(QuitGame);

    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(GameStart);
        quitButton.onClick.RemoveListener(QuitGame);

    }

    private void GameStart()
    {
        SceneManagerEx.Instance.ChangeScene(Define.SceneType.StoryScene);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
