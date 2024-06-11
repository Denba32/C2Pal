using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource effect;
    public AudioSource hitSound;

    private Dictionary<string, AudioClip> soundEffectClip = new Dictionary<string, AudioClip>();


    public float bgmVolume = 1.0f;
    public float effectVolume = 1.0f;

    protected override void Awake()
    {
        base.Awake();

        if(effect == null)
            effect = gameObject.AddComponent<AudioSource>();

        if (hitSound == null)
            hitSound = gameObject.AddComponent<AudioSource>();
        effect.loop = false;
        effect.playOnAwake = false;

        Init();
    }

    private void Init()
    {
        var data = ResourceManager.Instance.LoadAll<AudioClip>("Sounds/Effect");

        for(int i = 0; i < data.Length; i++)
        {
            soundEffectClip.Add(data[i].name, data[i]);
        }
    }


    public void Play(string clipName, Define.SoundType type)
    {
        if (soundEffectClip.TryGetValue(clipName, out AudioClip clip))
        {
            Play(clip, type);
        }
    }
    public void Play(AudioClip clip, Define.SoundType type)
    {
        if (type == Define.SoundType.Effect)
        {
            if (effect.isPlaying)
                effect.Stop();

            effect.PlayOneShot(clip, effectVolume);
        }
        else if (type == Define.SoundType.Hit)
        {
            if (hitSound.isPlaying)
                hitSound.Stop();

            hitSound.PlayOneShot(clip, effectVolume);
        }

    }
}
