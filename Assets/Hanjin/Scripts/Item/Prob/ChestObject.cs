using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestObject : GimmickObject
{
    [Header("Reward")]
    public ItemData reward;
    public int gold;
    public GameObject goldPrefab;
    
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
            else if(rewardType == Define.RewardType.Resource)
            {
                goldPrefab.SetActive(false);
            }
            else if(rewardType == Define.RewardType.Armor)
            {

            }
            else if(rewardType == Define.RewardType.Weapon)
            {

            }
            isActive = true;
            animator.SetBool(hashActive, true);
            if (aSource.isPlaying)
                aSource.Stop();
            aSource.PlayOneShot(openChest, 1f);
            GetComponent<Collider>().enabled = false;
        }
    }

    public ItemData Reward()
    {
        if(rewardType != Define.RewardType.Gold)
        {
            if (reward == null)
                return null;
            return reward;
        }
        return null;

    }

}
