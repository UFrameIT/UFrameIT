using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandling : MonoBehaviour, IDropHandler 
{
    GameObject current;
    public void OnDrop(PointerEventData eventData){
        Debug.Log(eventData.pointerDrag + "was dropped on "+ gameObject.name);
        Destroy(current);
        current = Instantiate(eventData.pointerDrag,Vector3.zero, Quaternion.identity);
        current.transform.SetParent(gameObject.transform, false);
    }

}
