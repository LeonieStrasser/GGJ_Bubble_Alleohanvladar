using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using UnityEngine.Serialization;
using GGJ_Cowboys;

public class BottleShaker : MonoBehaviour
{
    private InputAction 
        Cowboy_1_Stick, 
        Cowboy_2_Stick;
    private Vector2 
        lastC1Value = new(),
        lastC2Value = new();

    private float
        C1Velocity,
        smoothedC1VelocityChanges,
        C2Velocity,
        smoothedC2VelocityChanges;

    private List<float>
        C1VelChanges = new List<float>(),
        C2VelChanges = new List<float>();

    [FormerlySerializedAs("VelocityChangeSmooting")] [SerializeField, BoxGroup("Settings")] 
    private int VelocityChangeSmoothing = 3;

    [SerializeField, BoxGroup("Thresholds")]
    private float
        SmallShake = 15,
        MediumShake = 40,
        BigShake = 60;

    private ShakeState 
        C1_CurrentShake,
        C1_lastBiggestShake,
        C2_CurrentShake,
        C2_lastBiggestShake;

    public Action<Cowboy, ShakeState> ShakeEvent;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cowboy_1_Stick = InputSystem.actions.FindAction("Player/Cowboy_1_Stick");
        Cowboy_2_Stick = InputSystem.actions.FindAction("Player/Cowboy_2_Stick");

        for (int i = 0; i < VelocityChangeSmoothing; i++)
        {
            C1VelChanges.Add(0);
            C2VelChanges.Add(0);
        }
    }
    
    private void FixedUpdate()
    {
        StickReadOut();
        //Debug.Log($"C1 {smoothedC1VelocityChanges}");
        ShakeState c1Shake = EvaluateCurrentShake(smoothedC1VelocityChanges);
        RegisterShakes(c1Shake, ref C1_lastBiggestShake, Cowboy.Cowboy1);
        
        ShakeState c2Shake = EvaluateCurrentShake(smoothedC2VelocityChanges);
        RegisterShakes(c2Shake, ref C1_lastBiggestShake, Cowboy.Cowboy2);
    }

    //Tracks the veloctiy of the sticks and feeds the class velocity values.
    private void StickReadOut()
    {
        Vector2 Cowboy_1_Stick___Value = Cowboy_1_Stick.ReadValue<Vector2>();
        Vector2 Cowboy_2_Stick___Value = Cowboy_2_Stick.ReadValue<Vector2>();

        float C1delta = (Cowboy_1_Stick___Value - lastC1Value).magnitude;
        float C2delta = (Cowboy_2_Stick___Value - lastC2Value).magnitude;

        float lastC1Velocity = C1Velocity;
        float lastC2Velocity = C2Velocity;

        C1Velocity = C1delta / Time.fixedDeltaTime;
        C2Velocity = C2delta / Time.fixedDeltaTime;

        float c1VelocityChange = Mathf.Abs(C1Velocity - lastC1Velocity);
        C1VelChanges.Add(c1VelocityChange);
        C1VelChanges.RemoveRange(0,1);
        
        float c2VelocityChange = Mathf.Abs(C2Velocity - lastC2Velocity);
        C2VelChanges.Add(c2VelocityChange);
        C2VelChanges.RemoveRange(0,1);

        smoothedC1VelocityChanges = 0;
        foreach (var entry in C1VelChanges)
            smoothedC1VelocityChanges += entry;
        smoothedC1VelocityChanges /= VelocityChangeSmoothing;
        
        smoothedC2VelocityChanges = 0;
        foreach (var entry in C2VelChanges)
            smoothedC2VelocityChanges += entry;
        smoothedC2VelocityChanges /= VelocityChangeSmoothing;
        
        lastC1Value = Cowboy_1_Stick___Value;
        lastC2Value = Cowboy_2_Stick___Value;
    }
    
    private ShakeState EvaluateCurrentShake(float smoothedVelChange)
    {
        if (smoothedVelChange > BigShake)
            return ShakeState.Big;
        if (smoothedVelChange > MediumShake)
            return ShakeState.Medium;
        if (smoothedVelChange > SmallShake)
            return ShakeState.Small;
        
        return ShakeState.Rest;
    }

    private void RegisterShakes(ShakeState currentShake, ref ShakeState highestShake, Cowboy cowboy)
    {
        switch (currentShake)
        {
            case ShakeState.Small:
                if (highestShake < ShakeState.Small)
                    highestShake = ShakeState.Small;
                break;
            
            case ShakeState.Medium:
                if (highestShake < ShakeState.Medium)
                    highestShake = ShakeState.Medium;
                break;
            
            case ShakeState.Big:
                if (highestShake < ShakeState.Big)
                    highestShake = ShakeState.Big;
                break;
            
            default:
            case ShakeState.Rest:
                if (highestShake > ShakeState.Rest)
                {
                    Debug.Log($"SHAKE REGISTERED: {highestShake}");
                    ShakeEvent?.Invoke(cowboy, highestShake);
                    highestShake = ShakeState.Rest;
                }
                break;
        }
    }

    

    private void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(500,500), new Vector2(500,500)), smoothedC1VelocityChanges.ToString());
    }
}
