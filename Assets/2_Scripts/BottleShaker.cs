using UnityEngine;
using UnityEngine.InputSystem;

public class BottleShaker : MonoBehaviour
{
    private InputAction 
        Cowboy_1_Stick, 
        Cowboy_2_Stick;
    private Vector2 
        lastC1Value = new(),
        lastC2Value = new();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cowboy_1_Stick = InputSystem.actions.FindAction("Player/Cowboy_1_Stick");
        Cowboy_2_Stick = InputSystem.actions.FindAction("Player/Cowboy_2_Stick");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 Cowboy_1_Stick___Value = Cowboy_1_Stick.ReadValue<Vector2>();
        Vector2 Cowboy_2_Stick___Value = Cowboy_2_Stick.ReadValue<Vector2>();

        float C1delta = (Cowboy_1_Stick___Value - lastC1Value).magnitude;
        float C2delta = (Cowboy_2_Stick___Value - lastC2Value).magnitude;

        float C1Velocity = C1delta / Time.deltaTime;
        float C2Velocity = C2delta / Time.deltaTime;
        
        Debug.Log($"Stick Left: {Cowboy_1_Stick___Value}, vel: {C1Velocity}   Stick Right: {Cowboy_2_Stick___Value}, vel: {C2Velocity} @ {Time.frameCount}");

        lastC1Value = Cowboy_1_Stick___Value;
        lastC2Value = Cowboy_2_Stick___Value;
    }
}
