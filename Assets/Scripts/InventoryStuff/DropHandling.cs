using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandling : MonoBehaviour, IDropHandler 
{
    GameObject current;
    public Fact currentFact;

    public void OnDrop(PointerEventData eventData){
        
        var scrollFact = gameObject.GetComponent<RenderedScrollFact>();
        Debug.Log(eventData.pointerDrag.GetComponent<FactWrapper>().fact.Label+ " was dropped on "
            + gameObject.name+ " " +scrollFact.ID + "/" +
            ScrollDetails.ParameterDisplays.Count+" label: "+scrollFact.Label);
        
        Destroy(current);

        current = Instantiate(eventData.pointerDrag,Vector3.zero, Quaternion.identity);

        current.transform.SetParent(gameObject.transform, false);

        currentFact = eventData.pointerDrag.GetComponent<FactWrapper>().fact;
        Debug.Log("recieved Fact: " + currentFact.backendURI);

        CommunicationEvents.NewAssignmentEvent.Invoke();
    }

}
 