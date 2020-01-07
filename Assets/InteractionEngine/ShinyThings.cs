using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static CommunicationEvents;

public class ShinyThings : MonoBehaviour
{
    public WorldCursor Cursor;
    //Attributes for Highlighting of Facts when Mouse-Over
    private string selectableTag = "Selectable";
    private Transform lastFactSelection;
    public Material defaultMaterial;
    public Material highlightMaterial;


    //Attributes for simulating the drawing of a line/curve
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();

    private bool lineDrawingActivated;
    private bool curveDrawingActivated;
    private int curveDrawingVertexCount = 36;
    private LineFact curveDrawingStartLine;
    private Vector3 curveEndPoint;
    private Vector3 angleMiddlePoint;
    private float curveRadius;

    // Start is called before the first frame update
    public void Start()
    {
        if (Cursor == null) Cursor = GetComponent<WorldCursor>();
        CommunicationEvents.StartLineDrawingEvent.AddListener(ActivateLineDrawing);
        CommunicationEvents.StopLineDrawingEvent.AddListener(DeactivateLineDrawing);
        CommunicationEvents.StartCurveDrawingEvent.AddListener(ActivateCurveDrawing);
        CommunicationEvents.StopCurveDrawingEvent.AddListener(DeactivateCurveDrawing);
    }

    // Update is called once per frame
    public void Update()
    {
        //SELECTION-HIGHLIGHTING-PART
        //Check if a Fact was Hit

        RaycastHit Hit = Cursor.Hit;

        if (Hit.transform != null)
        {
            Transform selection = Hit.transform;

            //Set the last Fact unselected
            if (this.lastFactSelection != null)
            {
                //Invoke the EndHighlightEvent that will be handled in FactSpawner
                // CommunicationEvents.EndHighlightEvent.Invoke(this.lastFactSelection);
                OnMouseOverFactEnd(lastFactSelection);
                this.lastFactSelection = null;
            }

            //Set the Fact that was Hit as selected
            if (selection.CompareTag(selectableTag))
            {
                //Invoke the HighlightEvent that will be handled in FactSpawner
                this.lastFactSelection = selection;
                //CommunicationEvents.HighlightEvent.Invoke(selection);
                OnMouseOverFact(lastFactSelection);
            }
            //SELECTION-HIGHLIGHTING-PART-END

            //LineRendering-Part
            if (this.lineDrawingActivated)
                UpdateLineDrawing(Hit.point);
            else if (this.curveDrawingActivated)
                UpdateCurveDrawing(Hit.point);
        }


    }

    public void OnMouseOverFact(Transform selection)
    {
        Renderer selectionRenderer;
        selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            //Set the Material of the Fact, where the mouse is over, to a special one
            selectionRenderer.material = highlightMaterial;
        }
    }

    public void OnMouseOverFactEnd(Transform selection)
    {
        Renderer selectionRenderer;

        if (selection != null)
        {
            selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                //Set the Material of the fact back to default
                selectionRenderer.material = defaultMaterial;
            }
        }
    }

    public void ActivateLineDrawing(Fact startFact)
    {
        this.lineRenderer.positionCount = 2;

        lineRenderer.startWidth = 0.095f;
        lineRenderer.endWidth = 0.095f;
        //Set LineDrawing activated
        this.lineDrawingActivated = true;
        //Add the position of the Fact for the start of the Line
        linePositions.Add(startFact.Representation.transform.position);
        //The second point is the same point at the moment
        linePositions.Add(startFact.Representation.transform.position);

        this.lineRenderer.SetPosition(0, linePositions[0]);
        this.lineRenderer.SetPosition(1, linePositions[1]);

    }

    //Updates the second-point of the Line when First Point was selected in LineMode
    public void UpdateLineDrawing(Vector3 currentPosition)
    {
        this.linePositions[1] = currentPosition;
        this.lineRenderer.SetPosition(1, this.linePositions[1]);
    }

    //Deactivate LineDrawing so that no Line gets drawn when Cursor changes
    public void DeactivateLineDrawing(Fact startFact)
    {
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.lineDrawingActivated = false;
    }

    //Expect a LineFact here, so that it's possible to change between two possible StartPoints
    public void ActivateCurveDrawing(Fact startFact)
    {
        this.lineRenderer.positionCount = curveDrawingVertexCount;

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        //Set CurveDrawing activated
        this.curveDrawingActivated = true;

        curveDrawingStartLine = (LineFact)startFact;

        curveEndPoint = Cursor.transform.position;

        //Determine which point of the line is closer to the cursor and initialize angleMiddlePoint
        //angleMiddlePoint is needed for the Angle  Preview
        float Distance1 = (Facts.Find(x => x.Id == curveDrawingStartLine.Pid1).Representation.transform.position - curveEndPoint).magnitude;
        float Distance2 = (Facts.Find(x => x.Id == curveDrawingStartLine.Pid2).Representation.transform.position - curveEndPoint).magnitude;

        if (Distance1 >= Distance2)
        {
            angleMiddlePoint = Facts.Find(x => x.Id == curveDrawingStartLine.Pid2).Representation.transform.position;
            curveRadius = Distance2;
        }
        else
        {
            angleMiddlePoint = Facts.Find(x => x.Id == curveDrawingStartLine.Pid1).Representation.transform.position;
            curveRadius = Distance1;
        }
    }

    public void UpdateCurveDrawing(Vector3 currentPosition)
    {
        
        //Determine the Start-Point
        Vector3 startPoint = angleMiddlePoint + curveRadius * (currentPosition - angleMiddlePoint).normalized;
        //Determine the Center of Start-Point and End-Point 
        Vector3 tempCenterPoint = Vector3.Lerp(startPoint, curveEndPoint, 0.5f);
        Vector3 curveMiddlePoint = angleMiddlePoint + curveRadius * (tempCenterPoint - angleMiddlePoint).normalized;
        
        linePositions = new List<Vector3>();

        for (float ratio = 0; ratio <= 1; ratio += 1.0f / this.curveDrawingVertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPoint, curveMiddlePoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(curveMiddlePoint, curveEndPoint, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            linePositions.Add(bezierPoint);
        }

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
        
    }

    public void DeactivateCurveDrawing(Fact startFact)
    {
        
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.curveDrawingActivated = false;
    }
}
