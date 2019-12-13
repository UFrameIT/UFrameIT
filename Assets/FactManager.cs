using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;
public class FactManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        //Je nachdem ob erster oder der zweite Punkt angeklickt wurde behandeln

        //Wenn erster Punkt einen Point-Collider erwischt hat:
        //Linie aktivieren und Cursor folgen
        //Wenn erster Punkt keinen Point-Collider erwischt hat:
        //Nichts tun -> Evtl Hint einblenden

        //Wenn zweiter Punkt einen Point-Collider erwischt hat:
        //Event senden um GameObject-Line zu erzeugen
        //Wenn zweiter Punkt keinen Point-Collider erwischt hat:
        //Linie deaktivieren -> Evtl Hint einblenden

        //LayerMask for Points
        int layerMask = 1 << LayerMask.NameToLayer("Point"); //only hit Point

        //Wenn bereits der erste Punkt markiert wurde
        if (this.lineRendererActivated) //instead: bool variable....
        {
            //If a second Point was Hit
            if (Physics.Raycast(ray, out Hit, 30f, layerMask))
            {
                //Event for Creating the Line
                Vector3 point1 = this.linePositions[0];
                Vector3 point2 = Hit.transform.gameObject.transform.position;
                this.DeactivateLineRenderer();
                CommunicationEvents.AddLineEvent.Invoke(point1, point2);
                break;
            }
            //If no Point was hit
            else
            {
                //TODO: Hint that only a line can be drawn between already existing points
                this.DeactivateLineRenderer();
            }
        }
        //Wenn der erste Punkt noch nicht markiert wurde
        else
        {
            //Check if a Point was hit
            if (Physics.Raycast(ray, out Hit, 30f, layerMask))
            {
                //Set LineRenderer activated
                this.lineRendererActivated = true;
                //Add the position of the hit Point for the start of the Line
                Vector3 temp = Hit.transform.gameObject.transform.position;
                //temp += Vector3.up;

                linePositions.Add(temp);
                //The second point is the same point at the moment
                linePositions.Add(temp);
                this.lineRenderer.SetPosition(0, linePositions[0]);
                this.lineRenderer.SetPosition(1, linePositions[1]);
            }
            else
            {
                //TODO: Hint that only a line can be drawn between already existing points
            }
        }



    }


    public int GetFirstEmptyID()
    {

        for (int i = 0; i < Facts.Length; ++i)
        {
            if (Facts[i] == "")
                return i;
        }
        return Facts.Length - 1;

    }

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {
        switch (ActiveToolMode)
        {
            case ToolMode.MarkPointMode:
                //If MarkPointMode is activated we want to have the ability to mark the point
                //everywhere, independent of already existing facts
                foreach (GameObject GameObjectFact in GameObjectFacts)
                {
                    GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                }
                break;
            case ToolMode.CreateLineMode:
                //If CreateLineMode is activated we want to have the ability to select points for the Line
                //but we don't want to have the ability to select Lines or Angles
                foreach (GameObject GameObjectFact in GameObjectFacts)
                {
                    if (GameObjectFact.layer == LayerMask.NameToLayer("Line") || GameObjectFact.layer == LayerMask.NameToLayer("Angle"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.layer == LayerMask.NameToLayer("Point"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.CreateAngleMode:
                //If CreateAngleMode is activated we want to have the ability to select Lines for the Angle
                //but we don't want to have the ability to select Points or Angles
                foreach (GameObject GameObjectFact in GameObjectFacts)
                {
                    if (GameObjectFact.layer == LayerMask.NameToLayer("Point") || GameObjectFact.layer == LayerMask.NameToLayer("Angle"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.layer == LayerMask.NameToLayer("Line"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.DeleteMode:
                //If DeleteMode is activated we want to have the ability to delete every Fact
                //independent of the concrete type of fact
                foreach (GameObject GameObjectFact in GameObjectFacts)
                {
                    GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                }
                break;
            case ToolMode.ExtraMode:
                foreach (GameObject GameObjectFact in GameObjectFacts)
                {

                }
                break;



        }
    }

    public void OnHit(RaycastHit hit)
    {
        Debug.Log(CommunicationEvents.ActiveToolMode);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            //hit existing point, so delete it
            if (CommunicationEvents.ActiveToolMode == ToolMode.ExtraMode)
            {
                var menu = GameObject.Instantiate(SmartMenu);
                menu.GetComponent<Canvas>().worldCamera = Camera.main;
                menu.transform.SetParent(hit.transform);
                menu.transform.localPosition = Vector3.up - Camera.main.transform.forward;
            }
            else
            {
                char letter = hit.transform.gameObject.GetComponentInChildren<TextMeshPro>().text.ToCharArray()[0];
                int id = letter - 65;
                CommunicationEvents.RemoveEvent.Invoke(id);
            }

        }
        else
        {

            CommunicationEvents.AddPointEvent.Invoke(hit, GetFirstEmptyID());
        }
    }



}
