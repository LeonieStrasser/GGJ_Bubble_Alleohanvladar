using NaughtyAttributes;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    private GameManager myGameManager;



    [Header("Bottle Preasure")]
    [SerializeField] float preasureIncreaseSpeedMultiplyer = .5f;
    private float bottlePreasure;
    float BottlePreasure
    {
        get { return bottlePreasure; }
        set
        {
            OnPreasureChange(value);
            bottlePreasure = value;
        }


    }

    [HorizontalLine(color: EColor.Blue)]
    [Header("Preasure Feedback")]
    [SerializeField] BottleFeedbackTrigger[] feedbackMarker;

    [HorizontalLine(color: EColor.Blue)]
    [Header("Throwing")]
    [SerializeField] float flyingTime;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] AnimationCurve flyingSpeedCurve;
    [SerializeField] AnimationCurve yOffsetCurve;

    
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
        foreach (var feedbackMarker in feedbackMarker)
        {
            feedbackMarker.SetTriggerTime();
        }
    }

    private void Update()
    {
        if (myGameManager.Flying) { UpdateBottleFlyPosition(); }
    }

    #region preasure
    private void OnPreasureChange(float currentPreasue)
    {
        foreach (var feedbackMarker in feedbackMarker)
        {
            if (!feedbackMarker.triggered && feedbackMarker.triggerTime >= bottlePreasure)
            {
                feedbackMarker.feedbackEvent.Invoke();

                feedbackMarker.triggered = true;
            }
        }

    }

    private void ResetBottle()
    {
        BottlePreasure = 0;
        foreach (var feedbackMarker in feedbackMarker)
        {
            feedbackMarker.SetTriggerTime();
            feedbackMarker.triggered = false;
        }
    }


    public void IncreasePreasurebyValue(int increaseValue)
    {
        BottlePreasure = Mathf.Clamp(BottlePreasure + increaseValue * preasureIncreaseSpeedMultiplyer, 0, 100);
    }



    #endregion

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
        float value = flyingSpeedCurve.Evaluate(flyProgress);
        Vector3 lerpedPosition = Vector3.Lerp(currentStartPosition, currentTargetPosition, value); // + new Vector3(0, yOffsetCurve.Evaluate(flyProgress), 0);
        transform.position = lerpedPosition;

        flyTimer += Time.deltaTime;

        // if throw finished

        if (flyTimer >= flyingTime)
        {
            myGameManager.Flying = false;
            flyTimer = 0;

            transform.position = currentTargetPosition;
        }
    }

}
