using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static MenuScript;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public int SlotID; // 1 = success; 2 = failure; 3 = npc
    public GameObject animationprefab;
    public bool itemOnSlot = false;
    public string npcReaction;
    public AnimatorOverrideController npcController;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if(eventData.pointerDrag != null && SlotID == eventData.pointerDrag.GetComponent<DragAndDrop>().itemID)
        {
            if(SlotID == 3)
            {
                npcReaction = eventData.pointerDrag.GetComponent<DragAndDrop>().npcReaction;
                npcController = eventData.pointerDrag.GetComponent<DragAndDrop>().npcController;
            }
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.GetComponent<DragAndDrop>().droppedOnSlot = true;
            animationprefab = eventData.pointerDrag.GetComponent<DragAndDrop>().animationprefab;
            itemOnSlot = true;

        }
    }
}
