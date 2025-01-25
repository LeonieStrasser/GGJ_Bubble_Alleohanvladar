using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject DefaultSelectedMenuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(DefaultSelectedMenuButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
