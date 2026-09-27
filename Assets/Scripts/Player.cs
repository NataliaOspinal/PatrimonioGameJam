using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;

    public void PlayWalk(bool isWalking)
    {
        if (animator != null) animator.SetBool("isWalking", isWalking);
    }

    public void PlayTalk(bool isTalking)
    {
        if (animator != null) animator.SetBool("isTalking", isTalking);
    }

    public void FaceTarget(Vector2 targetPosition)
    {
        Vector3 escalaActual = transform.localScale;

        if (targetPosition.x < transform.position.x)
        {
            escalaActual.x = -Mathf.Abs(escalaActual.x);
        }
        else if (targetPosition.x > transform.position.x)
        {
            escalaActual.x = Mathf.Abs(escalaActual.x);
        }

        transform.localScale = escalaActual;
    }
}