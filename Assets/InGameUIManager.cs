using System;
using System.Collections;
using GGJ_Cowboys;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public Image Cowboy_1_Green_Lost;
    public Image Cowboy_2_Blue_Lost;

    public GameObject Player1_Green_Touched;
    public GameObject Player2_Blue_Touched;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cowboy_1_Green_Lost.gameObject.SetActive(false);
        Cowboy_2_Blue_Lost.gameObject.SetActive(false);
        
        Player1_Green_Touched.gameObject.SetActive(false);
        Player2_Blue_Touched.gameObject.SetActive(false);
        
        GameManager.Instance.OnGameWon += OnGameWon;
        GameManager.Instance.OnBottleTouched += OnBottleTouched;
    }

    private void OnBottleTouched(Cowboy newOwner)
    {
        switch (newOwner)
        {
            case Cowboy.Cowboy1:
                Player1_Green_Touched.gameObject.SetActive(true);
                Player2_Blue_Touched.gameObject.SetActive(false);
                break;
            case Cowboy.Cowboy2:
                Player1_Green_Touched.gameObject.SetActive(false);
                Player2_Blue_Touched.gameObject.SetActive(true);
                break;
            
            
            default:
            case Cowboy.None:
                Player1_Green_Touched.gameObject.SetActive(false);
                Player2_Blue_Touched.gameObject.SetActive(false);
                break;
        }
    }

    private void OnGameWon(Cowboy losingCowboy)
    {
        StartCoroutine(GameWonUIProgress(losingCowboy));
    }

    private IEnumerator GameWonUIProgress(Cowboy losingCowboy)
    {
        //wait for bottle explosion
        yield return new WaitForSeconds(2);
        
        //show overlay
        switch (losingCowboy)
        {
            case Cowboy.Cowboy1:
                Cowboy_1_Green_Lost.gameObject.SetActive(true);
                Cowboy_2_Blue_Lost.gameObject.SetActive(false);
                GameManager.Instance.Cowboy2.PlayState = CowboyState.Winning;
                CameraManager.Instance.LookAtPlayer1();
            break;
            
            case Cowboy.Cowboy2:
                Cowboy_1_Green_Lost.gameObject.SetActive(false);
                Cowboy_2_Blue_Lost.gameObject.SetActive(true);
                GameManager.Instance.Cowboy1.PlayState = CowboyState.Winning;
                CameraManager.Instance.LookAtPlayer2();
            break;
            
            default:
            case Cowboy.None:
                throw new ArgumentOutOfRangeException(nameof(losingCowboy), losingCowboy, null);
        }

        SoundCenter.Instance.PlayWinningSound();
        
        //wait for overlay
        yield return new WaitForSeconds(3);
        
        //Report back to game manager
        GameManager.Instance.ReturnToMenu();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWon -= OnGameWon;
        GameManager.Instance.OnBottleTouched -= OnBottleTouched;
    }
}
