using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BottleFeedbackTrigger
{
    [SerializeField, ReadOnly, BoxGroup("Debug")] public float triggerTime;
    [MinMaxSlider(0.0f, 100.0f)]
    public Vector2 triggerRange;
    public bool triggered = false;
    public UnityEvent feedbackEvent;

    public void SetTriggerTime()
    {
        triggerTime = Random.Range(triggerRange.x, triggerRange.y);
    }
}
