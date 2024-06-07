using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItem : ItemObject
{
    public new ResourceItemData data
    {
        get => (ResourceItemData)base.data;
        set => base.data = value;
    }

    public AudioSource aSource;


    public override string GetInteractPrompt()
    {
        if (data == null)
        {
            Debug.Log("해당 아이템 데이터가 존재하지 않습니다.");
            return base.GetInteractPrompt();
        }

        string str = $"{data.displayName}";
        return str;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        Debug.Log($"{data.displayName} 자원 줍기");
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.onAddItem?.Invoke();

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Level"))
            {
                if(aSource != null)
                {
                    if (aSource.isPlaying)
                        aSource.Stop();
                    aSource.PlayOneShot(data.dropSound, 0.5f);

                }
            }
        }
    }
    public override InteractableType GetInteractableType() => InteractableType.Pickup;
}