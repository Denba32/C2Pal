using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Consumable,
    Weapon,
    Armor,
    Resource
}

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
}