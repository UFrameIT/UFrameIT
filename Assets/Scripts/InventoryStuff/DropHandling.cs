using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandling : MonoBehaviour, IDropHandler, IPointerClickHandler
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
        //Set imageToChangeDefaultColor of current: Fix so that current won't take the color
        //the dragged item was having during animation
        current.GetComponent<ImageHintAnimation>().imageToChangeDefaultColor = eventData.pointerDrag.GetComponent<ImageHintAnimation>().imageToChangeDefaultColor;
        current.GetComponent<ImageHintAnimation>().ResetAnimation();

        current.transform.SetParent(gameObject.transform, false);

        currentFact = eventData.pointerDrag.GetComponent<FactWrapper>().fact;
        Debug.Log("recieved Fact: " + currentFact.backendURI);

        CommunicationEvents.NewAssignmentEvent.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Destroy(current);
        currentFact = null;
        CommunicationEvents.NewAssignmentEvent.Invoke();
    }

}
 