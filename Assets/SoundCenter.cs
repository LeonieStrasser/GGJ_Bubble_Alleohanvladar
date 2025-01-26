using System;
using GGJ_Cowboys;
using UnityEngine;

public class SoundCenter : MonoBehaviour
{
    public static SoundCenter Instance;

    private FMOD.Studio.EventInstance 
        smallShakeInstance,
        mediumShakeInstance,
        bigShakeInstance,
        bottleTossInstance,
        bottleCatchInstance,
        bottleExplosionInstance;

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
}
