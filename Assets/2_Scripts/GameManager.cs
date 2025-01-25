using System;
using System.Collections;
using GGJ_Cowboys;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
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

    
    
    public GameObject BottlePrefab;
    private Bottle _bottle;
    public Bottle Bottle
    {
        get => _bottle;
        set
        {
            _bottle = value;
            if (Bottle)
                Debug.Log("Linked Bottle in GameManager");
            else
                Debug.Log("Removed Bottle from GameManager");
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
            StartCowboyTurn(activeCowboy);
        }
    }

    private int tosses = 0;
    
    private bool flying;
    public bool Flying
    {
        get { return flying; }
        set
        {
            flying = value; 
            if(flying)
                CameraManager.Instance.LookAtBottle();
        }
    }
    
    //######################################################################################
    //#######################   Awake, Start and other set-up stuff  #######################
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

    public void SetCowboySlot(CowboyController askingCowboy)
    {
        switch (askingCowboy.player)
        {
            case Cowboy.Cowboy1:
                Cowboy1 = askingCowboy;
                break;
            case Cowboy.Cowboy2:
                Cowboy2 = askingCowboy;
                break;
        }

        if (Cowboy1 && Cowboy2)
        {
            //making sure the cowboys are right on start.
            Cowboy1.PlayerDude.SetActive(true);
            Cowboy1.CowboyModel.SetActive(false);
                
            Cowboy2.PlayerDude.SetActive(false);
            Cowboy2.CowboyModel.SetActive(true);
            SetUpBottle();
            
            CameraManager.Instance.LookAtPlayer1();
        }
            
    }

    private void SetUpBottle()
    {
        Debug.Log("Spawn bottle");
        GameObject bottleObj = Instantiate(BottlePrefab);
        Instance.Bottle = bottleObj.GetComponent<Bottle>();
        Bottle.position1 = Cowboy1.HandPoint;
        Bottle.position2 = Cowboy2.HandPoint;
        Bottle.SetStartPosition();
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
                Bottle = null;
                Cowboy1 = null;
                Cowboy2 = null;
                break;
            
            case GameState.PreGame:
                ActiveCowboy = Cowboy.None;
                
                StartCoroutine(PreGamePlaceholder());
                break;
            
            case GameState.InGame:
                //when going from pre game to game, start round
                if (oldState == GameState.PreGame)
                {
                    StartGame();
                }
                break;
            
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            
            case GameState.PostGame:
                ActiveCowboy = Cowboy.None;
                StartCoroutine(PostGamePlaceholder());
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
        yield return new WaitForSeconds(3);
        Instance.CurrentGameState = GameState.InGame;
    }
    
    private IEnumerator PostGamePlaceholder()
    {
        Debug.Log("Entered post game. here is space for animations and effects.");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("01_MainMenu");
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
                CameraManager.Instance.LookAtPlayer1();
                break;
            
            case Cowboy.Cowboy2:
                Debug.Log("Set Turn - Cowboy 2");
                Cowboy1.PlayState = CowboyState.Moving;
                Cowboy2.PlayState = CowboyState.Shaking;
                CameraManager.Instance.LookAtPlayer2();
                break;
        }
    }

    public void ReportBottleLanded()
    {
        switch (ActiveCowboy)
        {
            default:
            case Cowboy.None:
                Debug.LogError("Bottle landed while no cowboy was active. broken shit.");
                break;
            case Cowboy.Cowboy1:
                ActiveCowboy = Cowboy.Cowboy2;
                break;
            case Cowboy.Cowboy2:
                ActiveCowboy = Cowboy.Cowboy1;
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
