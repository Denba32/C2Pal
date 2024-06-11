using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public AudioSource aSource;

    public AudioClip audioClip;

    public Define.SoundType soundType;
    private void Awake()
    {
        TryGetComponent(out aSource);
    }

    private void Start()
    {
        if(aSource == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
        
        if(soundType == Define.SoundType.Bgm)
        {
            if(audioClip != null)
                aSource.clip = audioClip;
            aSource.loop = true;
            aSource.playOnAwake = true;
            aSource.volume = SoundManager.Instance.bgmVolume;
            aSource.Play();
        }
        else if(soundType == Define.SoundType.Effect)
        {
            aSource.volume = SoundManager.Instance.effectVolume;
        }

    }

    private void LateUpdate()
    {
        if (soundType == Define.SoundType.Bgm)
        {
            if(aSource != null)
            {
                aSource.volume = SoundManager.Instance.bgmVolume;

            }
        }
        else if (soundType == Define.SoundType.Effect)
        {
            if (aSource != null)
            {
                aSource.volume = SoundManager.Instance.effectVolume;

            }
        }
    }


}
