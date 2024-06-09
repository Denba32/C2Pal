using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftTable : MonoBehaviour, IInteractable
{
    public GameObject craftUI;

    public AudioSource aSource;
    public AudioClip startCraftSound;
    public AudioClip createSound;
    public InteractableType GetInteractableType() => InteractableType.Use;
    public string GetInteractPrompt()
    {
        return "제작하기";
    }

    public void OnInteract()
    {
        UIManager.Instance.ShowCraft();
    }
}
