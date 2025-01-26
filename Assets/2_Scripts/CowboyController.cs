using System;
using GGJ_Cowboys;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class CowboyController : MonoBehaviour
{
    public Cowboy player;
    public Transform HandPoint;

    public Animator ArmAnimator;
    
    public GameObject 
        PlayerDude,
        PlayerDudeLowerArm,
        PlayerDudeHand,
        CowboyModel,
        WinningCowboy;

    private Quaternion ArmStartRotation, HandStartRotation;
    private Vector2 ArmMoveSpanDeg = new Vector2(18, 24);
    
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

            switch (playState)
            {
                case CowboyState.Shaking:
                    ArmAnimator.speed = 1;
                    CowboyModel.SetActive(false);
                    PlayerDude.SetActive(true);
                    WinningCowboy.SetActive(false);
                    break;
                case CowboyState.Moving:
                    ArmAnimator.speed = 0;
                    CowboyModel.SetActive(true);
                    PlayerDude.SetActive(false);
                    WinningCowboy.SetActive(false);
                    break;
                
                case CowboyState.Winning:
                    CowboyModel.SetActive(false);
                    PlayerDude.SetActive(false);
                    WinningCowboy.SetActive(true);
                    break;
                
                default:
                case CowboyState.Idle:
                    ArmAnimator.speed = 0;
                    break;
            }
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
    [SerializeField, ReadOnly] 
    private float StickVelocity;
    
    [SerializeField]
    private float VelocityChangeToPressureBuiltUpFactor = 0.0037f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("No game manager to attach to. Delete this cowboy.");
            Destroy(gameObject);
            return;
        }

        ArmAnimator.speed = 0;
        ArmStartRotation = PlayerDudeLowerArm.transform.rotation;
        HandStartRotation = PlayerDudeHand.transform.localRotation;
        //assign self to Game manager slot and get relevant analog stick
        
        GameManager.Instance.SetCowboySlot(this);
        
        switch (player)
        {
            default:
            case Cowboy.None:
                break;
            case Cowboy.Cowboy1:
                AnalogStick = InputSystem.actions.FindAction(LeftStickPath);
                Toss = InputSystem.actions.FindAction(LeftTriggerPath);
                break;
            case Cowboy.Cowboy2:
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
            default:
            case CowboyState.Idle:
                // do nothing
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

        MoveArm();

        float lastStickVelocity = StickVelocity;
        Vector2 stickMoveDelta = StickValue - lastStickValue;
        StickVelocity = stickMoveDelta.magnitude / Time.fixedDeltaTime;
        float deltaVelocityChange = Mathf.Abs(StickVelocity - lastStickVelocity);
        GameManager.Instance.Bottle.AddPressureBuiltUp(deltaVelocityChange * VelocityChangeToPressureBuiltUpFactor);
        
        if (Mathf.Abs(lastStickValue.magnitude) < 0.75f && Mathf.Abs(StickValue.magnitude) > 0.75f)
        {
            if(StickVelocity > 50)
                SoundCenter.Instance.PlayBottleShake(GGJ_Cowboys.Shake.Big);
            else if (StickVelocity > 25)
                SoundCenter.Instance.PlayBottleShake(GGJ_Cowboys.Shake.Medium);
            else if (StickVelocity > 10)
                SoundCenter.Instance.PlayBottleShake(GGJ_Cowboys.Shake.Small);
        }
    }

    private void MoveArm()
    {
        //Move lower arm
        Vector3 oldArmEulerAngles = PlayerDudeLowerArm.transform.rotation.eulerAngles;
        Vector3 newArmEulerAngles = ArmStartRotation.eulerAngles +
                                    new Vector3(StickValue.y * -ArmMoveSpanDeg.y, 0, StickValue.x * ArmMoveSpanDeg.x);
        PlayerDudeLowerArm.transform.rotation = Quaternion.Euler(newArmEulerAngles);

        //move hand opposite
        Vector3 eulerAngleDif = (oldArmEulerAngles - newArmEulerAngles) * 2;
        Vector3 newEulerAngle = PlayerDudeHand.transform.localRotation.eulerAngles + eulerAngleDif;
        
        PlayerDudeHand.transform.localRotation = Quaternion.Euler(newEulerAngle);
        
        //lerp hand rot to target.
        Vector3 newTargetEulers = HandStartRotation.eulerAngles + new Vector3(StickValue.y * -ArmMoveSpanDeg.y, 0, StickValue.x * ArmMoveSpanDeg.x);
        PlayerDudeHand.transform.localRotation = Quaternion.Lerp(PlayerDudeHand.transform.localRotation, Quaternion.Euler(newTargetEulers), Time.fixedDeltaTime * 10);
    }

    private void Move()
    {
        //later
    }
    
    private void TossBottle(InputAction.CallbackContext obj)
    {
        //throwing only possible when bottle is also shakable i.e. in hand.
        if(PlayState != CowboyState.Shaking) return;
        
        switch (player)
        {
            case Cowboy.None:
                break;
            case Cowboy.Cowboy1:
                GameManager.Instance.Bottle.ThrowBottle(1);
                break;
            case Cowboy.Cowboy2:
                GameManager.Instance.Bottle.ThrowBottle(2);
                break;
        }
        
        //enter idle after throw. will be set to moving, once the bottle reached other cowboy.
        PlayState = CowboyState.Idle;

    }

    private void OnDestroy()
    {
        Toss.performed -= TossBottle;
        
        //assign self to Game manager slot and get relevant analog stick
        switch (player)
        {
            case Cowboy.Cowboy1:
                GameManager.Instance.Cowboy1 = null;
                break;
            case Cowboy.Cowboy2:
                GameManager.Instance.Cowboy2 = null;
                break;
        }
    }
}

public enum CowboyState
{
    Idle,
    Shaking,
    Winning,
    Moving
}
