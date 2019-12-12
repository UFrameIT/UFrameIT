using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorldCursor : MonoBehaviour
{
    private RaycastHit Hit;
    private Camera Cam;
    private ToolMode ActiveToolMode{get; set;}

    //Attributes for Highlighting of Facts when Mouse-Over
    private string selectableTag = "Selectable";
    private Transform lastFactSelection;

    //Attributes for simulating the drawing of a line
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    private bool lineRendererActivated;

    void Start()
    {

        Cam = Camera.main;
        //Set MarkPointMode as the default ActiveToolMode
        this.ActiveToolMode = ToolMode.ExtraMode;//ToolMode.MarkPointMode;
        CommunicationEvents.ToolModeChangedEvent.Invoke(this.ActiveToolMode);
        //TODO: we probably can configure these things to automatically trigger when the variable is changed...
        CommunicationEvents.ActiveToolMode = this.ActiveToolMode;
        //redundant for now, but we probably want to have the activetool mode available globally
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

       
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //only hit player
        layerMask = ~layerMask; //ignore Player

   

        if(Physics.Raycast(ray, out Hit, 30f, layerMask)){
            transform.position = Hit.point;
            transform.up = Hit.normal;
            transform.position += .01f * Hit.normal;

            //SELECTION-HIGHLIGHTING-PART
            //Check if a Fact was Hit
            Transform selection = Hit.transform;

            //Set the last Fact unselected
            if (this.lastFactSelection != null)
            {
                //Invoke the EndHighlightEvent that will be handled in FactSpawner
                CommunicationEvents.EndHighlightEvent.Invoke(this.lastFactSelection);
                this.lastFactSelection = null;
            }

            //Set the Fact that was Hit as selected
            if (selection.CompareTag(selectableTag))
            {
                //Invoke the HighlightEvent that will be handled in FactSpawner
                this.lastFactSelection = selection;
                CommunicationEvents.HighlightEvent.Invoke(selection);
            }
            //SELECTION-HIGHLIGHTING-PART-END

            CheckMouseButtons(ray);

            UpdateLineRenderer(transform.position);

        }
        else
        {
            transform.position = Cam.ScreenToWorldPoint(Input.mousePosition);
            transform.up = -Cam.transform.forward;
        }

        //Check if the ToolMode was switched
        CheckToolModeSelection();
        
    }

    //Deactivate LineRenderer so that no Line gets drawn when Cursor changes
    void DeactivateLineRenderer()
    {
        //Reset the first points
        this.lineRenderer.SetPosition(0, Vector3.zero);
        this.lineRenderer.SetPosition(1, Vector3.zero);
        if (linePositions.Count > 0)
            this.linePositions.Clear();
        this.lineRendererActivated = false;
    }

    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons(Ray ray)
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (this.ActiveToolMode)
            {
                case ToolMode.MarkPointMode:
                    //send HitEvent
                    CommunicationEvents.TriggerEvent.Invoke(Hit);
                    break;
                case ToolMode.ExtraMode:
                    //send HitEvent
                    CommunicationEvents.TriggerEvent.Invoke(Hit);
                    break;
                case ToolMode.DeleteMode:
                    //send HitEvent
                    CommunicationEvents.TriggerEvent.Invoke(Hit);
                    break;
                case ToolMode.CreateLineMode:
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
                    if (this.lineRendererActivated)
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

                    break;
            }
        }
    }

    //Updates the second-point of the Line when First Point was selected in LineMode
    void UpdateLineRenderer(Vector3 currentPosition)
    {
        if (this.ActiveToolMode == ToolMode.CreateLineMode)
        {
            if (this.lineRendererActivated)
            {
                this.linePositions[1] = currentPosition;
                this.lineRenderer.SetPosition(1, this.linePositions[1]);
            }
        }
    }

    void CheckToolModeSelection() {
        if (Input.GetButtonDown("ToolMode")) {
            //Change the ActiveToolMode dependent on which Mode was selected
            if ((int)this.ActiveToolMode == Enum.GetNames(typeof(ToolMode)).Length - 1)
            {
                this.ActiveToolMode = 0;
            }
            else {
                this.ActiveToolMode++;
            }
            CommunicationEvents.ActiveToolMode = this.ActiveToolMode;
            //Invoke the Handler for the Facts
            CommunicationEvents.ToolModeChangedEvent.Invoke(this.ActiveToolMode);
        }
    }

 

}
