using NaughtyAttributes;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    private GameManager myGameManager;



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

    [HorizontalLine(color: EColor.Blue)]
    [Header("Throwing")]
    [SerializeField] float flyingTime;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] AnimationCurve yOffsetCurve;
    [SerializeField] float hightMultiplyer = 1;
    [SerializeField] float SpeedCurveMultiplyer = 2;


    private float flyTimer;
    private Vector3 currentStartPosition;
    private Vector3 currentTargetPosition;

    private void Awake()
    {
        myGameManager = FindAnyObjectByType<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetBottle();
    }

    private void Update()
    {
        if (myGameManager.Flying) { UpdateBottleFlyPosition(); }
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

        myGameManager.Flying = true;
    }

    void UpdateBottleFlyPosition()
    {
        float flyProgress = flyTimer / flyingTime;

        // Werte aus der yOffsetCurve abrufen
        float yValue = yOffsetCurve.Evaluate(flyProgress);
        float yValueNext = yOffsetCurve.Evaluate(flyProgress + 0.01f); // Kleiner Schritt für Steigung
        float yCurveSlope = Mathf.Abs((yValueNext - yValue) / 0.01f); // Änderungsrate (Steigung)

        // Geschwindigkeit basierend auf der Steigung anpassen
        float adjustedFlySpeed = Mathf.Lerp(1f, SpeedCurveMultiplyer, yCurveSlope); // Anpassen des Speed-Faktors (Skalierung nach Bedarf)


        Vector3 lerpedPosition = Vector3.Lerp(currentStartPosition, currentTargetPosition, flyProgress) + new Vector3(0, yOffsetCurve.Evaluate(flyProgress) * hightMultiplyer, 0);
        transform.position = lerpedPosition;

        flyTimer += Time.deltaTime * adjustedFlySpeed;

        // if throw finished

        if (flyTimer >= flyingTime)
        {
            myGameManager.Flying = false;
            flyTimer = 0;

            transform.position = currentTargetPosition;
        }
    }
    #endregion

    #region exploding

    private void BottleExploding()
    {
        Debug.Log("Bottle exploaded by Preasure Value of " + preasureMaxLimit + ".");

        //melde dem Gamemanager dass explosion passiert is


    }

    #endregion
}
