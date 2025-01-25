using System;
using GGJ_Cowboys;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class CowboyController : MonoBehaviour
{
    public Cowboy player;
    
    [SerializeField, ReadOnly, BoxGroup("CowboyInfo")]
    private CowboyState playState = CowboyState.Idle;
    public CowboyState PlayState
    {
        get => playState;
        set
        {
            Debug.Log($"{name}'s state was set to {value}.");
            StickValue = Vector2.zero;
            playState = value;
        }
    }

    [SerializeField] 
    private string 
        LeftStickPath = "Player/Cowboy_1_Stick",
        LeftTriggerPath = "Player/Cowboy_1_Toss",
        RightStickPath = "Player/Cowboy_2_Stick",
        RightTriggerPath = "Player/Cowboy_2_Toss";

    private InputAction
        AnalogStick,
        Toss;

    public Action<Shake> OnShake;

    [SerializeField, ReadOnly] 
    private Vector2 StickValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("No game manager to attach to. Delete this cowboy.");
            Destroy(gameObject);
            return;
        }
        
        //assign self to Game manager slot and get relevant analog stick
        switch (player)
        {
            default:
            case Cowboy.None:
                break;
            case Cowboy.Cowboy1:
                GameManager.Instance.Cowboy1 = this;
                AnalogStick = InputSystem.actions.FindAction(LeftStickPath);
                Toss = InputSystem.actions.FindAction(LeftTriggerPath);
                break;
            case Cowboy.Cowboy2:
                GameManager.Instance.Cowboy2 = this;
                AnalogStick = InputSystem.actions.FindAction(RightStickPath);
                Toss = InputSystem.actions.FindAction(RightTriggerPath);
                break;
        }
        
        Toss.performed += TossBottle;
    }
    

    void FixedUpdate()
    {
        switch (PlayState)
        {
            case CowboyState.Idle:
                break;
            case CowboyState.Shaking:
                Shake();
                break;
            case CowboyState.Moving:
                Move();
                break;
        }
    }

    private void Shake()
    {
        Vector2 lastStickValue = StickValue;
        StickValue = AnalogStick.ReadValue<Vector2>();

        if (Mathf.Abs(lastStickValue.y) < 0.75f && Mathf.Abs(StickValue.y) > 0.75f)
        {
            OnShake?.Invoke(GGJ_Cowboys.Shake.Small);
            Debug.Log($"{name}'s stick is at {StickValue} @ Frame {Time.frameCount}");
        }
    }

    private void Move()
    {
        //later
    }
    
    private void TossBottle(InputAction.CallbackContext obj)
    {
        Debug.Log($"{name} started Toss.");
    }

    private void OnDestroy()
    {
        Toss.performed -= TossBottle;
    }
}

public enum CowboyState
{
    Idle,
    Shaking,
    Moving
}
