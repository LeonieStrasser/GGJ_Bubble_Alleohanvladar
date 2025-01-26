using System;
using GGJ_Cowboys;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Bottle : MonoBehaviour
{
    [Header("Bottle Preasure")]
    [SerializeField, ReadOnly, BoxGroup("Debug")] bool BottleExploded = false;
    
    [MinMaxSlider(0.0f, 100.0f)]
    public Vector2 maxPressureRange;
    
    [SerializeField, ReadOnly, BoxGroup("Debug")] private float pressureMaxLimit;
    [SerializeField] float preasureIncreaseSpeedMultiplyer = .5f;
    
    [SerializeField, ReadOnly, BoxGroup("Debug")] 
    private float currentBottlePressure;
    float CurrentBottlePressure
    {
        get { return currentBottlePressure; }
        set
        {
            currentBottlePressure = value;
            if(!BottleExploded)
                OnPreasureChange();
        }
    }

    /// <summary>
    /// This value gets stocked up by the velocity changes exerted on the bottle.
    /// It drains into the bottle pressure over time.
    /// The higher the built-up, the faster the draining.
    /// </summary>
    [SerializeField, Tooltip("This value gets stocked up by the velocity changes exerted on the bottle.\n It drains into the bottle pressure over time.\n The higher the built-up, the faster the draining.")]
    private float pressureBuiltUp;

    [SerializeField]
    private float pressureConversionRate = 10;
    
    [SerializeField]
    private float PressurePerThrow = 15;
    
    [HorizontalLine(color: EColor.Blue)]
    [Header("Preasure Feedback")]
    [SerializeField] BottleFeedbackTrigger[] feedbackMarker;
    [SerializeField] VisualEffect bubbleEffect1;
    [SerializeField] VisualEffect bubbleEffect2;
    [SerializeField] VisualEffect explosionVFX;

    [HorizontalLine(color: EColor.Blue)]
    [Header("Throwing")]
    [SerializeField] float flyingTime;
    [SerializeField] public Transform position1;
    [SerializeField] public Transform position2;
    [SerializeField] AnimationCurve yOffsetCurve;
    [SerializeField] float hightMultiplyer = 1;
    [SerializeField] float SpeedCurveMultiplyer = 2;


    private float flyTimer;
    private Vector3 currentStartPosition;
    private Vector3 currentTargetPosition;

    
    public Camera
        bottleCam_P1_to_P2;


    public void ActivateBottleCam()
    {
        bottleCam_P1_to_P2.gameObject.SetActive(true);
    }
    
    public void DeactivateBottleCam()
    {
        bottleCam_P1_to_P2.gameObject.SetActive(false);
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeactivateBottleCam();
    }
    

    public void SetStartPosition()
    {
        transform.position = position1.position;
        transform.rotation = position1.rotation;
        ResetBottle();
        //GameManager.Instance.Cowboy1.OnShake += OnShake;
        //GameManager.Instance.Cowboy2.OnShake += OnShake;
    }

    private void OnShake(Shake shakeStrength)
    {
        switch (shakeStrength)
        {
            case Shake.Small:
                //do stuff
                TryIncreasePreasurebyValue(1);
                break;
            case Shake.Medium:
                break;
            case Shake.Big:
                break;
            default:
            case Shake.Rest:
                throw new ArgumentOutOfRangeException(nameof(shakeStrength), shakeStrength, null);
        }
    }

    private void Update()
    {
        //do nothing if bottle already blew up
        if(BottleExploded) return;
        
        if (GameManager.Instance.Flying)
        {
            UpdateBottleFlyPosition();
        }
        else
        {
            RotateToActiveCowboyHandTransform();
        }

        IncreasePressureFromBuildUp();

        UpdateBubbleVFX();
    }

    private void RotateToActiveCowboyHandTransform()
    {
        if (GameManager.Instance.ActiveCowboy == Cowboy.Cowboy1)
        {
            transform.position = position1.position;
            transform.rotation = position1.rotation;
        }
        else
        {
            transform.position = position2.position;
            transform.rotation = position2.rotation;
        }
    }

    private void ResetBottle()
    {
        pressureMaxLimit = Random.Range(maxPressureRange.x, maxPressureRange.y);
        foreach (var feedbackMarker in feedbackMarker)
        {
            feedbackMarker.SetTriggerTime();
            feedbackMarker.triggered = false;
        }
        CurrentBottlePressure = 0; // muss hier am ende stehen!!!
    }
    #region pressure

    public void AddPressureBuiltUp(float addedBuildUp)
    {
        pressureBuiltUp += addedBuildUp;
    }

    private void IncreasePressureFromBuildUp()
    {
        float pressureBuiltUpFactor = pressureBuiltUp / 100f;
        float pressureConversion = Time.deltaTime * pressureConversionRate * pressureBuiltUpFactor;

        float oldPressureBuiltUp = pressureBuiltUp;
        pressureBuiltUp = Mathf.Clamp(pressureBuiltUp - pressureConversion, 0, Single.MaxValue);

        float pressureChange = oldPressureBuiltUp - pressureBuiltUp;
        
        CurrentBottlePressure += pressureChange;
    }
    
    private void OnPreasureChange()
    {
        foreach (var feedbackMarker in feedbackMarker)
        {
            if (!feedbackMarker.triggered && CurrentBottlePressure >= feedbackMarker.triggerTime)
            {
                feedbackMarker.feedbackEvent.Invoke();

                feedbackMarker.triggered = true;

                Debug.Log("Bottle FEEDBACK was triggered at Preasure Value " + feedbackMarker.triggerTime);
            }
        }
        
        //check for explosion
        if(CurrentBottlePressure > pressureMaxLimit)
            BottleExploding();

    }



    public bool TryIncreasePreasurebyValue(int increaseValue)
    {
        if (!BottleExploded)
        {
            CurrentBottlePressure = Mathf.Clamp(CurrentBottlePressure + increaseValue * preasureIncreaseSpeedMultiplyer, 0, 100);


            // Bottle exploading
            if (currentBottlePressure >= pressureMaxLimit)
            {
                
                
            }
            return true;
        }else {return false; }
    }

    [Button]
    public void DebugIncreasePreasure()
    {
        TryIncreasePreasurebyValue(5);
    }

    void UpdateBubbleVFX()
    {
        bubbleEffect1.SetFloat("Intensity", CurrentBottlePressure/100);
        bubbleEffect2.SetFloat("Intensity", CurrentBottlePressure/100);
    }


    #endregion

    #region fly
    // FLIIIIEGEN!!!!

    [Button]
    public void DebugThrow()
    {
        ThrowBottle(1);
    }

    public void ThrowBottle(int throwCowboyID) // cowboy 1 = 1 , cowboy 2 = 2
    {
        if (throwCowboyID == 1)
        {
            currentStartPosition = position1.position;
            currentTargetPosition = position2.position;
        }
        else if (throwCowboyID == 2)
        {
            currentStartPosition = position2.position;
            currentTargetPosition = position1.position;
        }
        else
        {
            Debug.LogWarning("ThrowCowboy hat die falsche ID!!!");
        }
        
        transform.rotation = Quaternion.Euler(Vector3.zero);

        pressureBuiltUp += PressurePerThrow;

        SoundCenter.Instance.PlayBottleToss();
        
        GameManager.Instance.Flying = true;
    }

    void UpdateBottleFlyPosition()
    {
        float flyProgress = flyTimer / flyingTime;

        // Werte aus der yOffsetCurve abrufen
        float yValue = yOffsetCurve.Evaluate(flyProgress);
        float yValueNext = yOffsetCurve.Evaluate(flyProgress + 0.01f); // Kleiner Schritt f�r Steigung
        float yCurveSlope = Mathf.Abs((yValueNext - yValue) / 0.01f); // �nderungsrate (Steigung)

        // Geschwindigkeit basierend auf der Steigung anpassen
        float adjustedFlySpeed = Mathf.Lerp(1f, SpeedCurveMultiplyer, yCurveSlope); // Anpassen des Speed-Faktors (Skalierung nach Bedarf)


        Vector3 lerpedPosition = Vector3.Lerp(currentStartPosition, currentTargetPosition, flyProgress) + new Vector3(0, yOffsetCurve.Evaluate(flyProgress) * hightMultiplyer, 0);
        transform.position = lerpedPosition;

        flyTimer += Time.deltaTime * adjustedFlySpeed;

        // if throw finished

        if (flyTimer >= flyingTime)
        {
            GameManager.Instance.Flying = false;
            flyTimer = 0;
            transform.position = currentTargetPosition;
            SoundCenter.Instance.PlayBottleCatch();
            GameManager.Instance.ReportBottleLanded();
        }
    }
    #endregion

    #region exploding

    private void BottleExploding()
    {
        BottleExploded = true;
        Debug.Log($"Bottle exploded by pressure Value of {pressureMaxLimit}. BuildUp was {pressureBuiltUp}");

        //melde dem Gamemanager dass explosion passiert is
        GameManager.Instance.ReportBottleExploded();
        SoundCenter.Instance.PlayBottleExplosion();

        GameObject newExplosion = Instantiate(explosionVFX.gameObject, explosionVFX.gameObject.transform.position, this.transform.rotation);
        newExplosion.GetComponent<VisualEffect>().Play();
        Destroy(newExplosion, 2);
        Destroy(gameObject);
    }

    #endregion

    private void OnDestroy()
    {
        //GameManager.Instance.Cowboy1.OnShake -= OnShake;
        //GameManager.Instance.Cowboy2.OnShake -= OnShake;
        GameManager.Instance.Bottle = null;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 200, Screen.height - 300 - pressureBuiltUp * 3, 100, pressureBuiltUp * 3), $"PBU: {pressureBuiltUp}");
        GUI.Box(new Rect(Screen.width -350,Screen.height - 300 -CurrentBottlePressure * 3, 100, CurrentBottlePressure * 3), $"CBP: {CurrentBottlePressure}");
    }
}
