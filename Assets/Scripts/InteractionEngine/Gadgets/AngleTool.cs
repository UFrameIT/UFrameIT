using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class AngleTool : Gadget
{

    //Variables for AngleMode distinction
    private bool angleModeIsFirstPointSelected = false;
    private PointFact angleModeFirstPointSelected = null;
    private bool angleModeIsSecondPointSelected = false;
    private PointFact angleModeSecondPointSelected = null;

    //Attributes for simulating the drawing of a curve
    private bool curveDrawingActivated;
    public WorldCursor Cursor;
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    public Material anglePreviewMaterial;

    //Vertices for the Curve
    private int curveDrawingVertexCount = 36;
    private Vector3 curveEndPoint;
    private Vector3 angleMiddlePoint;
    private float curveRadius;

    void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (this.Cursor == null)
            this.Cursor = GameObject.FindObjectOfType<WorldCursor>();

        this.UiName = "Angle Mode";
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    //Initialize Gadget when enabled AND activated
    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
        this.ResetGadget();
    }

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            PointFact tempFact = (PointFact)GlobalStatic.stage.factState[hit.transform.GetComponent<FactObject>().URI];

            //If two points were already selected and now the third point got selected
            if (this.angleModeIsFirstPointSelected && this.angleModeIsSecondPointSelected)
            {
                //Create AngleFact
                //Check if new Point is equal to one of the previous points -> if true -> cancel
                if (!(this.angleModeFirstPointSelected.Id == tempFact.Id || this.angleModeSecondPointSelected.Id == tempFact.Id))
                {
                    FactManager.AddAngleFact(((PointFact)this.angleModeFirstPointSelected).Id, ((PointFact)this.angleModeSecondPointSelected).Id, ((PointFact)tempFact).Id);
                }

                ResetGadget();
            }
            //If only one point was already selected
            else if (this.angleModeIsFirstPointSelected && !this.angleModeIsSecondPointSelected)
            {
                //Check if the 2 selected points are the same: If not
                if (this.angleModeFirstPointSelected.Id != tempFact.Id)
                {
                    this.angleModeIsSecondPointSelected = true;
                    this.angleModeSecondPointSelected = tempFact;

                    ActivateCurveDrawing();
                }
                else
                {
                    this.angleModeFirstPointSelected = null;
                    this.angleModeIsFirstPointSelected = false;
                }
            }
            //If no point was selected before
            else
            {
                //Save the first point selected
                this.angleModeIsFirstPointSelected = true;
                this.angleModeFirstPointSelected = tempFact;
            }
        }
        //No point was hit
        else
        {
            ResetGadget();

            //TODO: Hint that only an angle can be created between 3 already existing points
        }
    }

    void Update()
    {
        if (!this.isActiveAndEnabled) return;
        if (this.curveDrawingActivated)
            UpdateCurveDrawing(this.Cursor.transform.position);
    }

    private void ResetGadget()
    {
        this.angleModeIsFirstPointSelected = false;
        this.angleModeFirstPointSelected = null;
        this.angleModeIsSecondPointSelected = false;
        this.angleModeSecondPointSelected = null;
        DeactivateCurveDrawing();
    }

    //Expect a LineFact here, where Line.Pid2 will be the Basis-Point of the angle
    public void ActivateCurveDrawing()
    {
        //In AngleMode with 3 Points we want to draw nearly a rectangle so we add a startPoint and an Endpoint to this preview
        this.lineRenderer.positionCount = curveDrawingVertexCount + 2;
        this.lineRenderer.material = this.anglePreviewMaterial;

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        //Set CurveDrawing activated
        this.curveDrawingActivated = true;

        //curveEndPoint is a point on the Line selected, with some distance from point2
        curveEndPoint = angleModeSecondPointSelected.Point + 0.3f * (angleModeFirstPointSelected.Point - angleModeSecondPointSelected.Point).magnitude * (angleModeFirstPointSelected.Point - angleModeSecondPointSelected.Point).normalized;

        angleMiddlePoint = angleModeSecondPointSelected.Point;
        curveRadius = (curveEndPoint - angleModeSecondPointSelected.Point).magnitude;
    }

    public void UpdateCurveDrawing(Vector3 currentPosition)
    {
        //Determine the Start-Point
        Vector3 startPoint = angleMiddlePoint + curveRadius * (currentPosition - angleMiddlePoint).normalized;

        //Determine the Center of Start-Point and End-Point 
        Vector3 tempCenterPoint = Vector3.Lerp(startPoint, curveEndPoint, 0.5f);
        Vector3 curveMiddlePoint = angleMiddlePoint + curveRadius * (tempCenterPoint - angleMiddlePoint).normalized;

        linePositions = new List<Vector3>();
        //Start: AngleMiddlePoint -> FirstPoint of Curve
        linePositions.Add(angleModeSecondPointSelected.Point);

        for (float ratio = 0; ratio <= 1; ratio += 1.0f / this.curveDrawingVertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPoint, curveMiddlePoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(curveMiddlePoint, curveEndPoint, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            linePositions.Add(bezierPoint);
        }

        //End: LastPoint of Curve -> AngleMiddlePoint
        linePositions.Add(angleModeSecondPointSelected.Point);

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());

    }

    public void DeactivateCurveDrawing()
    {
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.curveDrawingActivated = false;
    }

}
