using System;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private void Awake()
    {
        //Debug
        if (!GameManager.Instance)
        {
            Debug.Log("No GameManager found. Create new.");
            GameObject debugGameManager = new GameObject("DebugGameManager");
            GameManager newGameManager = debugGameManager.AddComponent<GameManager>();
            newGameManager.startState = GameManager.GameState.PreGame;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //When this scene starts, move to preGame
        GameManager.Instance.CurrentGameState = GameManager.GameState.PreGame;

        SoundCenter.Instance.StartGameMusic();
    }
}
