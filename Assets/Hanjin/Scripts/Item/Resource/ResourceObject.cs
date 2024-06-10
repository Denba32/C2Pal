using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public Define.ResourceType resourceType;
    public ResourceItemData itemToGive;
    public int capacity;
    public int dropCount;
    
    public AudioSource aSource;

    public AudioClip hitSound;

    public void Gather(Vector3 hitPoint, int power)
    {
        if (aSource.isPlaying)
            aSource.Stop();
        aSource.PlayOneShot(hitSound, 1.0f);
        for (int i = 0; i < power; i++)
        {
            if (capacity <= 0) break;

            capacity -= 1;
        }

        if (capacity <= 0)
        {
            for(int i = 0; i < dropCount; i++)
            {
                GameObject go = Instantiate(itemToGive.dropPrefab, hitPoint - Vector3.back + Vector3.up, Quaternion.identity);
                go.name = itemToGive.dropPrefab.name;
            }

            Destroy(gameObject);
        }
    }
}
