using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string GameSceneName;
    public GameObject DefaultSelectedMenuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(DefaultSelectedMenuButton);
        GameManager.Instance.CurrentGameState = GameManager.GameState.Menu;
        
        SoundCenter.Instance.StartMenuMusic();
    }

    public void LoadGame()
    {
        SoundCenter.Instance.PlayUIClick();
        StartCoroutine(LoadGameAfterDelay());
    }

    private IEnumerator LoadGameAfterDelay()
    {
        Debug.Log("Started load scene with delay.");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        Debug.Log("Scene loaded.");
    }

    public void QuitGame()
    {
        SoundCenter.Instance.PlayUICancel();
        Application.Quit();
    }
}
