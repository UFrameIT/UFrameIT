using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static MenuScript;

public class DragAndDrop : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler , IDropHandler
{

    [SerializeField] private Canvas canvas;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    //Zusatz von Kommentar aus Video:
    private Vector3 defaultPos;
    public bool droppedOnSlot;
    public int itemID; // sagt welche art es ist (succes reaction, failure reaction oder npc reaction) // 1 = success; 2 = failure; 3 = npc
    public GameObject animationprefab;
    public GameObject respective_item_slot;
    public string npcReaction;
    public AnimatorOverrideController npcController;


    void Start()
    {
        defaultPos = transform.position;
    }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        defaultPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        eventData.pointerDrag.GetComponent<DragAndDrop>().droppedOnSlot = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        if(droppedOnSlot == false)
        {
            transform.position = defaultPos;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        if(droppedOnSlot == true)
        {
            droppedOnSlot = false;
            transform.position = defaultPos;
            respective_item_slot.GetComponent<ItemSlot>().itemOnSlot = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

}
