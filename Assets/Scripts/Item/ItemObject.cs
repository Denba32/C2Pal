using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;


public enum InteractableType
{
    None,
    Pickup,
    Use,
    Talk,
}


public interface IInteractable
{
    public InteractableType GetInteractableType();
    public string GetInteractPrompt();
    public void OnInteract();
}

public abstract class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;
    public AudioSource aSource;

    public virtual string GetInteractPrompt()
    {
        return "";
    }

    public virtual InteractableType GetInteractableType()
    {
        return InteractableType.None;
    }

    public virtual void OnInteract()
    {
        // CharacterManager.Instance.Player.itemData = data;
        // CharacterManager.Instance.Player.addItem?.Invoke();
        // Destroy(gameObject);
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Level"))
            {
                if (aSource != null)
                {
                    if (aSource.isPlaying)
                        aSource.Stop();
                    aSource.PlayOneShot(data.dropSound, 0.5f);

                }
            }
        }
    }
}