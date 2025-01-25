using NaughtyAttributes;
using UnityEngine;

public class Bottle : MonoBehaviour
{
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var feedbackMarker in feedbackMarker)
        {
            feedbackMarker.SetTriggerTime();
        }
    }

    

    private void OnPreasureChange(float currentPreasue)
    {
        foreach (var feedbackMarker in feedbackMarker)
        {
            if(!feedbackMarker.triggered && feedbackMarker.triggerTime >= bottlePreasure)
            {
                feedbackMarker.feedbackEvent.Invoke();

                feedbackMarker.triggered = true;
            }
        }

    }

    private void ResetBottle()
    {
        BottlePreasure = 0;
    }

    [Button]
    public void IncreasePreasurebyValue(int increaseValue = -1)
    {
        BottlePreasure += increaseValue * preasureIncreaseSpeedMultiplyer;
    }


}
