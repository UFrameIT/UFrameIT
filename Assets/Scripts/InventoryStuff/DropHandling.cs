using System.Collections;
using System.Collections.Generic;
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

        //TODO modularize this with functions when feature is approved


        //changes assigned label, used for later processing, if this should not be changed, use another variable for rhs data
        if (Scroll.FactOccurences[scrollFact.ID] != null)
        {
            scrollFact.Label = eventData.pointerDrag.GetComponent<FactWrapper>().fact.Label;
            Debug.Log(Scroll.FactOccurences[scrollFact.ID].Count);
            foreach (var id in Scroll.FactOccurences[scrollFact.ID])
            {
                //change occurences in scroll description
                if (id.Key == -1)
                {


                    string label = ScrollDetails.ScrollDescription;
                    label = label.Remove(id.Value, 1);
                    label = label.Insert(id.Value, scrollFact.Label);
                    ScrollDetails.ScrollDescription = label;
                }
                // and also in facts
                else
                {
                    var affectedFact = ScrollDetails.ParameterDisplays[id.Key].GetComponentInChildren<RenderedScrollFact>();
                    string label = affectedFact.Label;
                    label = label.Remove(id.Value, 1);
                    label = label.Insert(id.Value, scrollFact.Label);
                    affectedFact.Label = label;

                }
            }

        }


        Destroy(current);
        current = Instantiate(eventData.pointerDrag,Vector3.zero, Quaternion.identity);
        current.transform.SetParent(gameObject.transform, false);
        //PythagorasScript pythagorasScript = scrollShow.GetComponent<PythagorasScript>();
        currentFact = eventData.pointerDrag.GetComponent<FactWrapper>().fact;
        Debug.Log("recieved Fact: " + currentFact.backendURI);
        //pythagorasScript.putFact(gameObject.name, fact);
    }

}
 