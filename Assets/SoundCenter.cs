using System;
using FMOD.Studio;
using GGJ_Cowboys;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundCenter : MonoBehaviour
{
    public static SoundCenter Instance;

    private FMOD.Studio.EventInstance 
        smallShakeInstance,
        mediumShakeInstance,
        bigShakeInstance,
        bottleTossInstance,
        bottleCatchInstance,
        bottleExplosionInstance,
        menuMusicInstance,
        gameMusicInstance,
        UiClickInstance,
        UiCancelInstance,
        bottleSpinInstance;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            enabled = false;
    }

    public void PlayBottleShake(Shake intensity)
    {
        switch (intensity)
        {
            default:
            case Shake.Rest:
                //play nothing
                break;
            case Shake.Small:
                Debug.Log("Play sound: small shake");
                smallShakeInstance =
                    FMODUnity.RuntimeManager.CreateInstance("event:/Bottle_Shake/Bottle_Shake_Slow_Single");
                smallShakeInstance.start();
                smallShakeInstance.release();
                break;
            case Shake.Medium:
                Debug.Log("Play sound: med shake");
                mediumShakeInstance =
                    FMODUnity.RuntimeManager.CreateInstance("event:/Bottle_Shake/Bottle_Shake_Mid_Single");
                mediumShakeInstance.start();
                mediumShakeInstance.release();
                break;
            case Shake.Big:
                Debug.Log("Play sound: big shake");
                bigShakeInstance =
                    FMODUnity.RuntimeManager.CreateInstance("event:/Bottle_Shake/Bottle_Shake_Fast_Single");
                bigShakeInstance.start();
                bigShakeInstance.release();
                break;
        }
        
        if(GameManager.Instance.Bottle)
            GameManager.Instance.Bottle.PlayShakeVFX();
    }

    public void PlayBottleToss()
    {
        Debug.Log("Play sound: toss");
        bottleTossInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Beer_Throw/Beer_Throw_Fast");
        bottleTossInstance.start();
        bottleTossInstance.release();
    }
    
    public void PlayBottleCatch()
    {
        Debug.Log("Play sound: catch");
        bottleCatchInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Cowboy_Cloth_WithSheriff/Cowboy_Cloth_WithSheriff");
        bottleCatchInstance.start();
        bottleCatchInstance.release();
    }


    public void PlayBottleExplosion()
    {
        bottleExplosionInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Explosion/Explosion");
        bottleExplosionInstance.start();
        bottleExplosionInstance.release();
    }
    
    public void StartMenuMusic()
    {
        menuMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Menu_Music");
        menuMusicInstance.start();

        StopGameMusic();
    }

    public void StopMenuMusic()
    {
        menuMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        menuMusicInstance.release();
    }
    
    public void StartGameMusic()
    {
        gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Game_Music");
        gameMusicInstance.start();

        StopMenuMusic();
    }

    public void StopGameMusic()
    {
        gameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        gameMusicInstance.release();
    }

    public void PlayWinningSound()
    {
        StopGameMusic();
        
        bottleExplosionInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Winning/Winning");
        bottleExplosionInstance.start();
        bottleExplosionInstance.release();
    }

    public void PlayUIClick()
    {
        UiClickInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UI/UI_Click");
        UiClickInstance.start();
        UiClickInstance.release();
    }
    
    public void PlayUICancel()
    {
        UiCancelInstance = FMODUnity.RuntimeManager.CreateInstance("event:/UI/UI_Cancel");
        UiCancelInstance.start();
        UiCancelInstance.release();
    }

    public void StartBottleFlying()
    {
        int rnd = Random.Range(0, 2);
        if (rnd == 0)
        {
            bottleSpinInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Bottle_Spin/Bottle_Spin_Fast_Loop");
        }
        else
        {
            bottleSpinInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Bottle_Spin/Bottle_Spin_Slow_Loop");
        }
        
        
        bottleSpinInstance.start();
    }

    public void StopBottleFlying()
    {
        bottleSpinInstance.stop(STOP_MODE.ALLOWFADEOUT);
        bottleSpinInstance.release();
    }
    
}
