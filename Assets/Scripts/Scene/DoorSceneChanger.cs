using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSceneChanger : MonoBehaviour, IInteractable
{
    public InteractableType GetInteractableType() => InteractableType.Use;
    public string GetInteractPrompt() => "성 밖으로\n나가기";

    public void OnInteract()
    {
        SceneManagerEx.Instance.ChangeScene(Define.SceneType.MainScene);
    }
}
