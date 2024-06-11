using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : GimmickObject
{
    public Define.LeverType leverType;
    public AudioClip leverSound;

    [Header("Target Info")]
    public GameObject influentialTarget = null;
    public Animator targetAnimator;

    private Coroutine co_Lever;
    private bool isPlaying = false;

    public CinemachineVirtualCamera virtualCamera;

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
        if(leverType == Define.LeverType.Normal)
        {
            isPlaying = true;
            isActive = !isActive;
            animator.SetBool(hashActive, isActive);
            if (aSource.isPlaying)
                aSource.Stop();
            aSource.PlayOneShot(leverSound, SoundManager.Instance.effectVolume);

            targetAnimator.SetBool("isActive", isActive);

            yield return CoroutineHelper.WaitForSeconds(2.0f);

            isPlaying = false;

            yield break;
        }

        else if(leverType == Define.LeverType.Special)
        {
            GameManager.Instance.GamePause();
            UIManager.Instance.ShowMainSceneUI(false);
            isPlaying = true;
            isActive = !isActive;
            virtualCamera.enabled = true;
            animator.SetBool(hashActive, isActive);
            if (aSource.isPlaying)
                aSource.Stop();
            aSource.PlayOneShot(leverSound, SoundManager.Instance.effectVolume);

            targetAnimator.SetBool("isActive", isActive);

            yield return CoroutineHelper.WaitForSeconds(5.0f);
            
            virtualCamera.enabled = false;
            UIManager.Instance.ShowMainSceneUI(true);
            GameManager.Instance.GameStart();

            isPlaying = false;

            yield break;
        }

    }
}
