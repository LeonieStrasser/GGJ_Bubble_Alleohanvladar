using System;
using System.Collections;
using GGJ_Cowboys;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public Image Cowboy_1_Green_Lost;
    public Image Cowboy_2_Blue_Lost;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cowboy_1_Green_Lost.gameObject.SetActive(false);
        Cowboy_2_Blue_Lost.gameObject.SetActive(false);
        
        GameManager.Instance.OnGameWon += OnGameWon;
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
            break;
            case Cowboy.Cowboy2:
                Cowboy_1_Green_Lost.gameObject.SetActive(false);
                Cowboy_2_Blue_Lost.gameObject.SetActive(true);
            break;
            
            default:
            case Cowboy.None:
                throw new ArgumentOutOfRangeException(nameof(losingCowboy), losingCowboy, null);
        }
        
        //wait for overlay
        yield return new WaitForSeconds(3);
        
        //Report back to game manager
        GameManager.Instance.ReturnToMenu();
    }
}
