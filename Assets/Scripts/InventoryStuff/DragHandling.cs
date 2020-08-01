using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandling : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 StartingPosition;
    private bool dragged = false;
    public void OnDrag(PointerEventData eventData){
        if(! dragged ){
            StartingPosition = transform.localPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            dragged = true;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.localPosition = StartingPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true ;
        dragged = false;

    } 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
