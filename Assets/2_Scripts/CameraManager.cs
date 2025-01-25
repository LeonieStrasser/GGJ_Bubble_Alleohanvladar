using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraPlayer1;
    [SerializeField] private GameObject cameraPlayer2;
    [SerializeField] private Camera 
        cameraBottle_P1_to_P2,
        cameraBottle_P2_to_P1;

    public static CameraManager Instance;

    private void Awake()
    {
        if(!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /*//I know its dirty af but I am exhausted
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            LookAtBottle();
            Debug.Log("I pressed C!");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            LookAtPlayer1();
            Debug.Log("I pressed X!");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            LookAtPlayer2();
            Debug.Log("I pressed V!");
        }
    }*/

    //if a player ends their turn, other cameras are turned off and we look through the bottlecamera
    public void LookAtBottle()
    {
        Debug.Log("Switched to Bottle Camera");
        cameraPlayer1.SetActive(false);
        cameraPlayer2.SetActive(false);
        GameManager.Instance.Bottle.ActivateBottleCam();
    }

    public void LookAtPlayer1()
    {
        Debug.Log("Switched to Player 1 Camera");
        cameraPlayer1.SetActive(true);
        cameraPlayer2.SetActive(false);
        GameManager.Instance.Bottle.DeactivateBottleCam();
    }

    public void LookAtPlayer2()
    {
        Debug.Log("Switched to Player 2 Camera");
        cameraPlayer1.SetActive(false);
        cameraPlayer2.SetActive(true);
        GameManager.Instance.Bottle.DeactivateBottleCam();
    }



}
