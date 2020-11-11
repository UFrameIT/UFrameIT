using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandling : MonoBehaviour, IDropHandler 
{
    GameObject current;
    public Fact currentFact;
    public DropHandling associatedDropHandling;

    public void OnDrop(PointerEventData eventData){
        
        var scrollFact = gameObject.GetComponent<RenderedScrollFact>();
        Debug.Log(eventData.pointerDrag.GetComponent<FactWrapper>().fact.Label+ " was dropped on "
            + gameObject.name+ " " +scrollFact.ID + "/" +
            ScrollDetails.ParameterDisplays.Count+" label: "+scrollFact.Label);
        
        Destroy(current);
        Destroy(associatedDropHandling.current);

        current = Instantiate(eventData.pointerDrag,Vector3.zero, Quaternion.identity);
        associatedDropHandling.current = Instantiate(current);

        current.transform.SetParent(gameObject.transform, false);
        associatedDropHandling.current.transform.SetParent(associatedDropHandling.gameObject.transform, false);

        currentFact = eventData.pointerDrag.GetComponent<FactWrapper>().fact;
        Debug.Log("recieved Fact: " + currentFact.backendURI);
    }

}
 