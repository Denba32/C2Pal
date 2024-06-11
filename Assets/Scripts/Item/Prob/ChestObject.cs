using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestObject : GimmickObject
{
    [Header("Reward")]
    public ItemData reward;
    public int gold;
    
    public GameObject goldPrefab;

    public Transform dropPosition;
    
    public AudioClip openChest;
    public Define.RewardType rewardType;


    public override string GetInteractPrompt()
    {
        if(!gameObject.activeInHierarchy)
        {
            return base.GetInteractPrompt();
        }

        return "상자 열기";
    }
    public override void OnInteract()
    {
        base.OnInteract();
        
        if(!isActive)
        {
            if(rewardType == Define.RewardType.Gold)
            {
                goldPrefab.SetActive(true);
            }
            else
            {
                goldPrefab.SetActive(false);
                GameObject go = Instantiate(reward.dropPrefab);
                if(go.TryGetComponent(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }
                go.transform.position = dropPosition.position;
            }

            isActive = true;
            animator.SetBool(hashActive, true);
            if (aSource.isPlaying)
                aSource.Stop();
            aSource.PlayOneShot(openChest, SoundManager.Instance.effectVolume);
            GetComponent<Collider>().enabled = false;
        }
    }

}
