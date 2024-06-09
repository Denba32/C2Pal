using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : GimmickObject
{
    public AudioClip leverSound;

    [Header("Target Info")]
    public GameObject influentialTarget = null;
    public Animator targetAnimator;

    private Coroutine co_Lever;
    private bool isPlaying = false;

    private void OnEnable()
    {
        isPlaying = false;
        isActive = false;
    }

    public override string GetInteractPrompt()
    {
        if(isActive)
        {
            return "레버 올리기";
        }
        else
        {
            return "레버 당기기";
        }
    }

    public override void OnInteract()
    {
        base.OnInteract();

        if(!isPlaying)
        {
            if (co_Lever != null)
            {
                StopCoroutine(co_Lever);
            }
            co_Lever = StartCoroutine(ActiveLever());
        }

    }

    private IEnumerator ActiveLever()
    {
        isPlaying = true;
        isActive = !isActive;
        animator.SetBool(hashActive, isActive);
        if (aSource.isPlaying)
            aSource.Stop();
        aSource.PlayOneShot(leverSound, 0.5f);

        targetAnimator.SetBool("isActive", isActive);

        yield return CoroutineHelper.WaitForSeconds(2.0f);

        isPlaying = false;

        yield break;
    }
}
