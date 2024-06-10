using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : GimmickObject
{
    public AudioClip doorOpen;
    public AudioClip doorClose;

    private Coroutine co_Door;

    [SerializeField]
    private bool isPlaying = false;
    public override string GetInteractPrompt()
    {
        if(!gameObject.activeInHierarchy)
        {
            return base.GetInteractPrompt();

        }
        if (isActive)
        {
            return "문 닫기";
        }
        else
        {
            return "문 열기";
        }

    }
    public override void OnInteract()
    {
        base.OnInteract();

        if(!isPlaying)
        {
            if (co_Door != null)
            {
                StopCoroutine(co_Door);
            }

            co_Door = StartCoroutine(ActiveDoor());
        }

    }

    private IEnumerator ActiveDoor()
    {
        if (gameObject.activeInHierarchy)
        {
            isActive = !isActive;
            isPlaying = true;


            animator.SetBool(hashActive, isActive);
            if (aSource.isPlaying)
                aSource.Stop();

            if (isActive)
                aSource.PlayOneShot(doorOpen, SoundManager.Instance.effectVolume);
            else
                aSource.PlayOneShot(doorClose, SoundManager.Instance.effectVolume);


            while ((animator.GetCurrentAnimatorStateInfo(0).IsName("Open") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
                || (animator.GetCurrentAnimatorStateInfo(0).IsName("Close") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f))
            {
                yield return null;
            }

            isPlaying = false;
            yield break;
        }
    }
}
