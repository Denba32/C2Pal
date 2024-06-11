using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSceneChanger : MonoBehaviour, IInteractable
{
    public InteractableType GetInteractableType() => InteractableType.Use;
    public string GetInteractPrompt() => "�� ������\n������";

    public void OnInteract()
    {
        SceneManagerEx.Instance.ChangeScene(Define.SceneType.MainScene);
    }
}
