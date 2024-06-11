using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickObject : MonoBehaviour, IInteractable
{
    public Animator animator;
    public AudioSource aSource;

    protected readonly int hashActive = Animator.StringToHash("isActive");

    protected bool isActive = false;


    private void OnEnable()
    {
        isActive = false;
    }

    public InteractableType GetInteractableType() => InteractableType.Use;

    public virtual string GetInteractPrompt()
    {
        return "";
    }

    public virtual void OnInteract() { }
}
