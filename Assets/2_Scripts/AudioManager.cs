using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    public MysteriousTickingNoise myTickNoise;
    public TypeSoundGenerator myTypeSound;
    public TextEnterSoundGenerator myTextEnter;

    // Start is called before the first frame update
    void Awake()
    {
        // Wenn schon ein Audiomanager Existiert, mach ihn kaputt
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);



        myTickNoise = FindObjectOfType<MysteriousTickingNoise>();
        myTypeSound = FindObjectOfType<TypeSoundGenerator>();
        myTextEnter = FindObjectOfType<TextEnterSoundGenerator>();
        foreach (var item in sounds)
        {
            item.mySource = gameObject.AddComponent<AudioSource>();
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
            item.mySource.loop = item.MyLoop;
        }


    }

    private void Start()
    {
        foreach (var item in sounds)
        {
            if (item.playFromStart)
            {
                Play(item.name);
            }
        }
    }

    public void Play(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToPlay.randomizePitch)
        {
            soundToPlay.mySource.pitch = RandomizePitch(soundToPlay.minMaxPitch);
        }
        soundToPlay.mySource.Play();
    }

    public void Stop(string clipName)
    {
        // STop SOund mit dem Namen
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        soundToPlay.mySource.Stop();
    }

    public void PlayStackable(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }




        // Kreiere eine neue Audiosource, damit sich nix gegenseitig abschneidet
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = soundToPlay.myClip;
        newSource.volume = soundToPlay.myVolume;
        newSource.pitch = soundToPlay.myPitch;
        newSource.loop = soundToPlay.MyLoop;
        if (soundToPlay.randomizePitch)
        {
            newSource.pitch = RandomizePitch(soundToPlay.minMaxPitch);
        }
        newSource.Play();

        // Verzögert die Zerstörung der hinzugefügten AudioSource-Komponente, nachdem der Sound abgespielt wurde.
        Destroy(newSource, soundToPlay.myClip.length);
    }

    private float RandomizePitch(Vector2 minMaxPitch)
    {
        float randomPitch = UnityEngine.Random.Range(minMaxPitch.x, minMaxPitch.y);
        return randomPitch;
    }
}