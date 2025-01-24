using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DebugPlayClip : MonoBehaviour
{
    public string soundToPlay;

    [Button]
   void PlaySoundInEditor()
    {
        AudioManager.instance.Play(soundToPlay);
    }

    [Button]
    void PlayStackableSoundInEditor()
    {
        AudioManager.instance.PlayStackable(soundToPlay);
    }
}
