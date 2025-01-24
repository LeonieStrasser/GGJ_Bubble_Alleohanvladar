using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip myClip;

    [Range(0f, 1f)]
    public float myVolume;
    public bool MyLoop;
    public bool playFromStart;

    // Pich Randomize
    public bool randomizePitch;

    [Range(.1f, 3f)] [HideIf("randomizePitch")] public float myPitch;

    [MinMaxSlider(0f, 3f)]
    [ShowIf("randomizePitch")]
    public Vector2 minMaxPitch;

    [HideInInspector]
    public AudioSource mySource;

}