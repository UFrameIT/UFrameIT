using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandling : MonoBehaviour, IDropHandler 
{
    GameObject current;
    public Fact currentFact;
    public void OnDrop(PointerEventData eventData){
        Debug.Log(eventData.pointerDrag + "was dropped on "+ gameObject.name);
        Destroy(current);
        current = Instantiate(eventData.pointerDrag,Vector3.zero, Quaternion.identity);
        current.transform.SetParent(gameObject.transform, false);
        //PythagorasScript pythagorasScript = scrollShow.GetComponent<PythagorasScript>();
        currentFact = eventData.pointerDrag.GetComponent<FactWrapper>().fact;
        Debug.Log("recieved Fact: " + currentFact.backendURI);
        //pythagorasScript.putFact(gameObject.name, fact);
    }

}
 