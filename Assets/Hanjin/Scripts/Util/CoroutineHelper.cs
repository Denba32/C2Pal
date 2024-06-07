using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static class CoroutineHelper
{
    private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    private static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;

        if(!_waitForSeconds.TryGetValue(seconds, out wfs))
            _waitForSeconds.Add(seconds, wfs = new UnityEngine.WaitForSeconds(seconds));

        return wfs;
    }
}
