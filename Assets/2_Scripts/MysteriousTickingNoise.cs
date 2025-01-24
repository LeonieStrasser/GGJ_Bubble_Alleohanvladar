using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MysteriousTickingNoise : MonoBehaviour
{
    public Sound tickingSound;
    [MinMaxSlider(0, 1)] public Vector2 volumeRangeOverLivetimeTick;
    public Sound neonPsyc;
    [MinMaxSlider(0, 1)] public Vector2 volumeRangeOverLivetimeNeon;
    public AnimationCurve neonSoundCurve;


    private void Awake()
    {
        neonPsyc.mySource = gameObject.AddComponent<AudioSource>();
        neonPsyc.mySource.clip = neonPsyc.myClip;
        neonPsyc.mySource.volume = neonPsyc.myVolume;
        neonPsyc.mySource.pitch = neonPsyc.myPitch;
        neonPsyc.mySource.loop = neonPsyc.MyLoop;

        tickingSound.mySource = gameObject.AddComponent<AudioSource>();
        tickingSound.mySource.clip = tickingSound.myClip;
        tickingSound.mySource.volume = tickingSound.myVolume;
        tickingSound.mySource.pitch = tickingSound.myPitch;
        tickingSound.mySource.loop = tickingSound.MyLoop;
    }

    private void Start()
    {
        neonPsyc.mySource.Play();
        tickingSound.mySource.Play();
        UpdateTickingVolume(0);
    }

    /// <summary>
    /// roundProcess beschreibt den Prozentanteil des Runden Progresses (0 = runde hat grade erst angefangen, 1 = Runde ist vorbei)
    /// </summary>
    /// <param name="roundProcess"></param>
    public void UpdateTickingVolume(float roundProcess)
    {
        if (roundProcess > 1)
        { Debug.LogWarning("Der roundProcess ist größer als 1 ihr Deppen! Er ist " + roundProcess, gameObject); }
        else if (roundProcess < 0)
        { Debug.LogWarning("Der roundProcess ist kleiner als 0 ihr Deppen! Er ist " + roundProcess, gameObject); }

        // Stellen Sie sicher, dass roundProcess zwischen 0 und 1 liegt.
        roundProcess = Mathf.Clamp(roundProcess, 0f, 1f);

        float minVolume = volumeRangeOverLivetimeTick.x;
        float maxVolume = volumeRangeOverLivetimeTick.y;
        float minVolumeNeon = volumeRangeOverLivetimeNeon.x;
        float maxVolumeNeon = volumeRangeOverLivetimeNeon.y;

        // Lerp (Interpolation) zwischen minVolume und maxVolume basierend auf roundProcess.
        float newVolumeTick = Mathf.Lerp(minVolume, maxVolume, roundProcess / 1f);
        float time = Mathf.Lerp(minVolumeNeon, maxVolumeNeon, roundProcess / 1f);
        float newVolumeNeon = neonSoundCurve.Evaluate(time);

        // Setzen Sie die Lautstärke der AudioSource auf den neuen Wert.
        tickingSound.mySource.volume = newVolumeTick;
        neonPsyc.mySource.volume = newVolumeNeon;
    }

    [Space(20)]
    // DEBUG
    [Range(0, 1)] public float debugRoundProcess;

    [Button]
    void DebugUpdateTickVolume()
    {
        UpdateTickingVolume(debugRoundProcess);
    }


}
