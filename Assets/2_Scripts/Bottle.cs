using System;
using GGJ_Cowboys;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Bottle : MonoBehaviour
{
    [Header("Bottle Preasure")]
    [MinMaxSlider(0.0f, 100.0f)]
    public Vector2 maxPreasureRange;
    [SerializeField, ReadOnly, BoxGroup("Debug")] private float preasureMaxLimit;
    [SerializeField] float preasureIncreaseSpeedMultiplyer = .5f;
    [SerializeField, ReadOnly, BoxGroup("Debug")] private float bottlePreasure;
    float BottlePreasure
    {
        get { return bottlePreasure; }
        set
        {
            OnPreasureChange(value);
            bottlePreasure = value;
        }
    }
    [SerializeField, ReadOnly, BoxGroup("Debug")] bool BottleExploded = false;


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
        GameManager.Instance.Bottle = this;
        DeactivateBottleCam();
    }
    

    public void SetStartPosition()
    {
        transform.position = position1.position;
        transform.rotation = position1.rotation;
        ResetBottle();
        GameManager.Instance.Cowboy1.OnShake += OnShake;
        GameManager.Instance.Cowboy2.OnShake += OnShake;
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
        if(BottleExploded) return;
        if (GameManager.Instance.Flying)
        {
            UpdateBottleFlyPosition();
        }
        else
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

        UpdateBubbleVFX();
    }

    private void ResetBottle()
    {
        preasureMaxLimit = Random.Range(maxPreasureRange.x, maxPreasureRange.y);
        foreach (var feedbackMarker in feedbackMarker)
        {
            feedbackMarker.SetTriggerTime();
            feedbackMarker.triggered = false;
        }
        BottlePreasure = 0; // muss hier am ende stehen!!!
    }
    #region preasure
    private void OnPreasureChange(float currentPreasue)
    {
        foreach (var feedbackMarker in feedbackMarker)
        {
            if (!feedbackMarker.triggered && currentPreasue >= feedbackMarker.triggerTime)
            {
                feedbackMarker.feedbackEvent.Invoke();

                feedbackMarker.triggered = true;

                Debug.Log("Bottle FEEDBACK was triggered at Preasure Value " + feedbackMarker.triggerTime);
            }
        }

    }



    public bool TryIncreasePreasurebyValue(int increaseValue)
    {
        if (!BottleExploded)
        {
            BottlePreasure = Mathf.Clamp(BottlePreasure + increaseValue * preasureIncreaseSpeedMultiplyer, 0, 100);


            // Bottle exploading
            if (bottlePreasure >= preasureMaxLimit)
            {
                BottleExploding();
                BottleExploded = true;
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
        bubbleEffect1.SetFloat("Intensity", BottlePreasure/100);
        bubbleEffect2.SetFloat("Intensity", BottlePreasure/100);
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
            GameManager.Instance.ReportBottleLanded();
        }
    }
    #endregion

    #region exploding

    private void BottleExploding()
    {
        Debug.Log("Bottle exploaded by Preasure Value of " + preasureMaxLimit + ".");

        //melde dem Gamemanager dass explosion passiert is
        GameManager.Instance.CurrentGameState = GameManager.GameState.PostGame;

        GameObject newExplosion = Instantiate(explosionVFX.gameObject, explosionVFX.gameObject.transform.position, this.transform.rotation);
        Destroy(newExplosion, 2);
        Destroy(gameObject);
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.Instance.Cowboy1.OnShake -= OnShake;
        GameManager.Instance.Cowboy2.OnShake -= OnShake;
        GameManager.Instance.Bottle = null;
    }
}
