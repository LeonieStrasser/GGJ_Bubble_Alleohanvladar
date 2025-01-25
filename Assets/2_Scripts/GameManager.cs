using System;
using System.Collections;
using GGJ_Cowboys;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] 
    public GameState startState = GameState.Menu;
    
    [SerializeField, ReadOnly, BoxGroup("GameInfo")]
    private GameState currentGameState = GameState.Menu;
    public GameState CurrentGameState
    {
        get => currentGameState;
        set
        {
            if (value == currentGameState)
            {
                Debug.Log($"Tried to set same GameState {value}.");
                return;
            }
            Debug.Log($"GameState changing from {currentGameState} to {value}.");
            SetGameState(value, currentGameState);
            currentGameState = value;
            
        }
    }
    
    [SerializeField, ReadOnly, BoxGroup("GameInfo")]
    public CowboyController 
        Cowboy1, 
        Cowboy2;
    
    
    [SerializeField, ReadOnly, BoxGroup("GameInfo")]
    private Cowboy activeCowboy = Cowboy.None;
    public Cowboy ActiveCowboy
    {
        get => activeCowboy;
        set
        {
            if(value == activeCowboy) return;
            activeCowboy = value;
            StartCowboyTurn(activeCowboy);
        }
    }

    private int tosses = 0;
    
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
        //prevent deletion
        DontDestroyOnLoad(this);
        
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
        //sets chosen state form inspector
        CurrentGameState = startState;
    }
    
    //######################################################################################
    //################################   Game Functions   ##################################
    //######################################################################################

    /// <summary>
    /// This function allows for set actions to be performed, when the game exits or enters specific game states.
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="oldState"></param>
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
                ActiveCowboy = Cowboy.None;
                StartCoroutine(PreGamePlaceholder());
                break;
            
            case GameState.InGame:
                //when going from pre game to game, start round
                if (oldState == GameState.PreGame)
                    StartGame();
                break;
            
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            
            case GameState.PostGame:
                ActiveCowboy = Cowboy.None;
                break;
        }
    }

    private void StartGame()
    {
        Debug.Log("Start Game");
        ActiveCowboy = Cowboy.Cowboy1;
        tosses = 0;
    }

    private IEnumerator PreGamePlaceholder()
    {
        Debug.Log("Started pre game. here is space for animations and effects.");
        yield return new WaitForSeconds(6);
        Instance.CurrentGameState = GameState.InGame;
    }

    private void StartCowboyTurn(Cowboy newActiveCowboy)
    {
        switch (newActiveCowboy)
        {
            default:
            case Cowboy.None:
                Cowboy1.PlayState = CowboyState.Idle;
                Cowboy2.PlayState = CowboyState.Idle;
                break;
            
            case Cowboy.Cowboy1:
                Debug.Log("Set Turn - Cowboy 1");
                Cowboy1.PlayState = CowboyState.Shaking;
                Cowboy2.PlayState = CowboyState.Moving;
                break;
            
            case Cowboy.Cowboy2:
                Debug.Log("Set Turn - Cowboy 2");
                Cowboy1.PlayState = CowboyState.Moving;
                Cowboy2.PlayState = CowboyState.Shaking;
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
