using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : MonoBehaviour, IInteractable
{
    public InteractableType GetInteractableType() => InteractableType.Use;

    public string GetInteractPrompt()
    {
        return "제작하기";
    }

    public void OnInteract()
    {

    }
}
