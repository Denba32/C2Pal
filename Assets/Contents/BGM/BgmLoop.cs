using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmLoop : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Space를 누른 경우 BGM loop On/Off
        if (Input.GetKeyDown(KeyCode.Space))
            audioSource.loop = !audioSource.loop;

        /* Space를 누른 경우 BGM mute On/Off
        if (Input.GetKeyDown(KeyCode.Space))
            audioSource.mute = !audioSource.mute;
        */

    }
}
