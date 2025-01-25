using System;
using GGJ_Cowboys;
using UnityEngine;

public class CowboyController : MonoBehaviour
{
    public Cowboy player;
    public CowboyState playState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("No game manager to attach to. Delete this cowboy.");
        }
        
        switch (player)
        {
            default:
            case Cowboy.None:
                break;
            case Cowboy.Cowboy1:
                GameManager.Instance.Cowboy1 = this;
                break;
            case Cowboy.Cowboy2:
                GameManager.Instance.Cowboy2 = this;
                break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum CowboyState
{
    Idle,
    Shaking,
    Moving
}
