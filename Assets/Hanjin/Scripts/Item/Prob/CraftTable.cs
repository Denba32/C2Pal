using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : MonoBehaviour, IInteractable
{
    public InteractableType GetInteractableType() => InteractableType.Use;

    public string GetInteractPrompt()
    {
        return "�����ϱ�";
    }

    public void OnInteract()
    {

    }
}
