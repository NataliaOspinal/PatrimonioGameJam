using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Transform player;

    [Header("Área Caminable de la Sala Actual")]
    public Collider2D walkableArea; 

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.GetComponent<ItemData>() != null)
            {
                ItemData clickedItem = hit.collider.GetComponent<ItemData>();
                InteractWithItem(clickedItem);
            }
            else
            {
                WalkToPoint(clickPosition);
            }
        }
    }

    private void WalkToPoint(Vector2 targetPosition)
    {
        if (walkableArea != null)
        {
            targetPosition = walkableArea.ClosestPoint(targetPosition);
        }

        StopAllCoroutines();
        StartCoroutine(MoveToPoint(targetPosition));
    }

    private void InteractWithItem(ItemData item)
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndInteract(item));
    }

    private IEnumerator MoveAndInteract(ItemData item)
    {
        Vector2 safePoint = item.goToPoint.position;
        if (walkableArea != null) safePoint = walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(MoveToPoint(safePoint));

        Debug.Log("Llegó al item " + item.gameObject.name);
    }

    public IEnumerator MoveToPoint(Vector2 point)
    {
        Vector2 positionDifference = point - (Vector2)player.position;
        while (positionDifference.magnitude > moveAccuracy)
        {
            player.Translate(moveSpeed * positionDifference.normalized * Time.deltaTime);
            positionDifference = point - (Vector2)player.position;
            yield return null;
        }
        player.position = point;
    }
}