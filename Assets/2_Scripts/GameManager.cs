using System;
using GGJ_Cowboys;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField, ReadOnly, BoxGroup("GameInfo")]
    private GameState currentGameState = GameState.Menu;
    public GameState CurrentGameState
    {
        get => currentGameState;
        set
        {
            if (value == currentGameState)
            {
                Debug.LogWarning($"Tried to set same GameState {value}. This shouldn't be happening.");
                return;
            }
            SetGameState(value, currentGameState);
            currentGameState = value;
            
        }
    }
    
    [SerializeField, ReadOnly, BoxGroup("GameInfo")]
    private Cowboy activeCowboy = Cowboy.None;
    public Cowboy ActiveCowboy
    {
        get => activeCowboy;
        set
        {
            if(value == activeCowboy) return;
            activeCowboy = value;
        }
    }
    private bool flying;
    public bool Flying
    {
        get { return flying; }
        set { flying = value; }
    }


    //######################################################################################
    //#################################   Awake & Start   ##################################
    //######################################################################################
    private void Awake()
    {
        SetSingleton();
    }

    private void SetSingleton()
    {
        if (Instance)
        {
            Debug.LogWarning($"GameManager instance is already occupied. This instance will be deleted.");
            Destroy(this);
            return;
        }

        Instance = this;
        Debug.Log("GameManager instance was set.");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    //######################################################################################
    //################################   Game Functions   ##################################
    //######################################################################################

    private void SetGameState(GameState newState, GameState oldState)
    {
        //on leaving state
        switch (oldState)
        {
            case GameState.Menu:
                break;
            case GameState.PreGame:
                break;
            case GameState.InGame:
                break;
            case GameState.Paused:
                Time.timeScale = 1;
                break;
            case GameState.PostGame:
                break;
        }
        
        //on entering state
        switch (newState)
        {
            case GameState.Menu:
                ActiveCowboy = Cowboy.None;
                break;
            case GameState.PreGame:
                ActiveCowboy = Cowboy.Cowboy1;
                break;
            case GameState.InGame:
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.PostGame:
                ActiveCowboy = Cowboy.None;
                break;
        }
    }
    
    //######################################################################################
    //####################################   Utility   #####################################
    //######################################################################################
    

    public enum GameState
    {
        Menu,
        PreGame,
        InGame,
        Paused,
        PostGame
    }
}
