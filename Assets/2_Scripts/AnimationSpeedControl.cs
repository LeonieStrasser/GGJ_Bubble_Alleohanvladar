using UnityEngine;

[ExecuteInEditMode]
public class AnimationSpeedControl : MonoBehaviour
{
    // Public variable für die Geschwindigkeit, die im Editor angepasst werden kann
    [Range(0f, 5f)]
    public float animationSpeed = 1f;

    private Animator animator;

    void Start()
    {
        // Animator des GameObjects holen
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Die Geschwindigkeit des Animators in Echtzeit anpassen
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }
}