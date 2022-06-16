using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandling : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 StartingPosition;
    private Transform StartingParent;
    private bool dragged = false;
    public void OnDrag(PointerEventData eventData)
    {
        if (!dragged)
        {
            StartingPosition = transform.localPosition;
            StartingParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            dragged = true;
        }
        transform.position = Input.mousePosition;

        // display dragged object in front of all other ui
        transform.parent = GetComponentInParent<Canvas>().transform;
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.parent = StartingParent;
        transform.localPosition = StartingPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        dragged = false;

    }
}
