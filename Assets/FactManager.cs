using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;
public class FactManager : MonoBehaviour
{



    public GameObject SmartMenu;
    private Stack<int> NextEmptyStack = new Stack<int>();

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);

        CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);//we also need the listener here at the moment so we can react to UI delete events

        NextEmptyStack.Push(0);
  
    }


    void AddLineFact(int pid1, int pid2, int id)
    {
       Facts.Insert(id, new LineFact
        {
            Id = id,
            Pid1 = pid1,
            Pid2 = pid2
        });
    }

    void AddAngleFact(int pid1, int pid2, int pid3, int id)
    {
        Facts.Insert(id, new AngleFact
        {
            Id = id,
            Pid1 = pid1,
            Pid2 = pid2,
            Pid3 = pid3
        });
    }


    PointFact AddPointFact(RaycastHit hit, int id)
    {
       
        Facts.Insert(id, new PointFact
        {
            Id = id,
            Point = hit.point
        });

        return Facts[id] as PointFact;
    }

    void DeleteFact(Fact fact)
    {
       
        NextEmptyStack.Push(fact.Id);
        Facts.RemoveAt(fact.Id);
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
        /*
        //Wenn bereits der erste Punkt markiert wurde
        if (this.lineRendererActivated) //instead: bool variable....
        {
            //If a second Point was Hit
            if (Physics.Raycast(ray, out Hit, 30f, layerMask)) //instead: another hitevent, refer to OnHit
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

        */

    }


    public int GetFirstEmptyID()
    {

        /* for (int i = 0; i < Facts.Length; ++i)
         {
             if (Facts[i] == "")
                 return i;
         }
         return Facts.Length - 1;*/

        int id = NextEmptyStack.Pop();
        if (NextEmptyStack.Count == 0)
            NextEmptyStack.Push(id + 1);

     
        return id;


    }

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {
        switch (ActiveToolMode)
        {
            case ToolMode.MarkPointMode:
                //If MarkPointMode is activated we want to have the ability to mark the point
                //everywhere, independent of already existing facts
                foreach (Fact fact in Facts)
                {
                   GameObject gO = fact.Representation;
                   gO.GetComponentInChildren<Collider>().enabled = false;
                }
                break;
            case ToolMode.CreateLineMode:
                //If CreateLineMode is activated we want to have the ability to select points for the Line
                //but we don't want to have the ability to select Lines or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO.layer == LayerMask.NameToLayer("Line") || gO.layer == LayerMask.NameToLayer("Angle"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Point"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.CreateAngleMode:
                //If CreateAngleMode is activated we want to have the ability to select Lines for the Angle
                //but we don't want to have the ability to select Points or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO.layer == LayerMask.NameToLayer("Point") || gO.layer == LayerMask.NameToLayer("Angle"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Line"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.DeleteMode:
                //If DeleteMode is activated we want to have the ability to delete every Fact
                //independent of the concrete type of fact
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    gO.GetComponentInChildren<Collider>().enabled = true;
                }
                break;
            case ToolMode.ExtraMode:
                foreach (Fact fact in Facts)
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
                CommunicationEvents.RemoveFactEvent.Invoke(Facts[id]);
            }

        }
        else
        {
            PointFact fact = AddPointFact(hit, GetFirstEmptyID());
            CommunicationEvents.AddFactEvent.Invoke(fact);
        }
    }



}
