using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class LotTool : Gadget
    //constructs a Perpendicular between a Line and a Point
{
    //Variables for LineMode distinction
    private bool LotModeIsPointSelected = false;
    private bool LotModeIsLineSelected = false;
    private AbstractLineFact LotModeLineSelected = null;
    private PointFact LotModeIntersectionPoint = null;
    private PointFact LotModeLinePointA = null;
    private RaycastHit LotModeLineHit;

    //Attributes for simulating the drawing of a line
    private bool lineDrawingActivated;
    public WorldCursor Cursor;
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    public Material linePreviewMaterial;

    void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (this.Cursor == null)
            this.Cursor = GameObject.FindObjectOfType<WorldCursor>();

        this.UiName = "Lot Mode";
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    //Initialize Gadget when enabled AND activated
    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
        this.ResetGadget();
    }

    void OnDisable()
    {
        this.ResetGadget();
    }

    public override void OnHit(RaycastHit hit)
    {
        void CreateRayAndAngles(string pidIntersectionPoint, string pidLotPoint, bool samestep)
        {
            FactManager.AddRayFact(pidIntersectionPoint, pidLotPoint, samestep);

            //TODO: create at all? / for all points on basline?
            FactManager.AddAngleFact(
                this.LotModeLineSelected.Pid1 == pidIntersectionPoint ? this.LotModeLineSelected.Pid2 : this.LotModeLineSelected.Pid1,
                pidIntersectionPoint, pidLotPoint, true);
        }

        if (!this.isActiveAndEnabled) return;

        //If LotPoint is on baseLine
        if (this.LotModeIsPointSelected && (hit.transform.gameObject.layer == LayerMask.NameToLayer("Default") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Tree")))
        {
            Vector3 LotPoint = Math3d.ProjectPointOnLine(hit.point, this.LotModeLineSelected.Dir, this.LotModeIntersectionPoint.Point);

            //TODO: which normal?
            CreateRayAndAngles(this.LotModeIntersectionPoint.URI, FactManager.AddPointFact(LotPoint, hit.normal).URI, true);
            this.ResetGadget();
        }

        //If baseline already selected and point selected
        else if (this.LotModeIsLineSelected && !this.LotModeIsPointSelected && hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            PointFact tempFact = LevelFacts[hit.transform.GetComponent<FactObject>().URI] as PointFact;

            Vector3 intersectionPoint = Math3d.ProjectPointOnLine(this.LotModeLinePointA.Point, this.LotModeLineSelected.Dir, tempFact.Point);

            if (intersectionPoint == tempFact.Point) // Vector3.operator== tests for almost Equal()
            {   //TempFact is on baseLine
                this.LotModeIsPointSelected = true;
                this.LotModeIntersectionPoint = tempFact;
                return;
            }

            //TODO: test Facts existance
            //add Facts
            var intersectionId = FactManager.AddPointFact(intersectionPoint, this.LotModeLineHit.normal).URI;

            if(this.LotModeLineSelected is RayFact) //Add OnLineFact only on Ray not Line
                FactManager.AddOnLineFact(intersectionId, this.LotModeLineSelected.URI, true);

            CreateRayAndAngles(intersectionId, tempFact.URI, true);
            this.ResetGadget();
        }

        //If nothing yet selected
        else if (!this.LotModeIsLineSelected
            && (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ray")
            || hit.transform.gameObject.layer == LayerMask.NameToLayer("Line")))
        {
            Fact tempFact = LevelFacts[hit.transform.GetComponent<FactObject>().URI];

            //Activate LineDrawing for preview
            this.LotModeIsLineSelected = true;
            this.LotModeLineSelected = tempFact as AbstractLineFact;
            this.LotModeLinePointA = (PointFact) LevelFacts[this.LotModeLineSelected.Pid1];
            this.LotModeLineHit = hit;
            this.ActivateLineDrawing();
        }

        //unexpected usage
        else
        {
            if (this.LotModeIsLineSelected)
            {
                //Deactivate LineDrawing and first point selection
                this.ResetGadget();
            }
        }
    }

    void Update()
    {
        if (!this.isActiveAndEnabled)
            return;
        if (this.lineDrawingActivated)
            UpdateLineDrawing();
    }

    private void ResetGadget()
    {
        this.LotModeIsLineSelected = false;
        this.LotModeLineSelected = null;

        this.LotModeLinePointA = null;

        this.LotModeIsPointSelected = false;
        this.LotModeIntersectionPoint = null;
        //TODO? reset?
        //this.LotModeLineHit;
        DeactivateLineDrawing();
    }

    private void ActivateLineDrawing()
    {
        this.lineRenderer.positionCount = 3;
        this.lineRenderer.material = this.linePreviewMaterial;

        lineRenderer.startWidth = 0.095f;
        lineRenderer.endWidth = 0.095f;
        //Set LineDrawing activated
        this.lineDrawingActivated = true;

        //start at curser
        linePositions.Add(this.Cursor.transform.position);
        //Project curser perpendicular on Line for intersection-point
        linePositions.Add(Math3d.ProjectPointOnLine(this.LotModeLinePointA.Point, this.LotModeLineSelected.Dir, this.linePositions[0]));
        //end at Point on the line (i.c.o. projection beeing outside a finite line)
        linePositions.Add(Math3d.ProjectPointOnLine(this.LotModeLinePointA.Point, this.LotModeLineSelected.Dir, this.LotModeLineHit.point));

        this.lineRenderer.SetPosition(0, linePositions[0]);
        this.lineRenderer.SetPosition(1, linePositions[1]);
        this.lineRenderer.SetPosition(2, linePositions[2]);

    }

    //Updates the points of the Lines when baseLine was selected in LineMode
    private void UpdateLineDrawing()
    {
        if (this.LotModeIsPointSelected)
        {
            this.linePositions[1] = this.LotModeIntersectionPoint.Point;
            this.linePositions[0] = Math3d.ProjectPointOnLine(this.Cursor.transform.position, this.LotModeLineSelected.Dir, this.linePositions[1]);
        }
        else
        {
            this.linePositions[0] = this.Cursor.transform.position;
            this.linePositions[1] = Math3d.ProjectPointOnLine(this.LotModeLinePointA.Point, this.LotModeLineSelected.Dir, this.linePositions[0]);
        }

        this.lineRenderer.SetPosition(0, this.linePositions[0]);
        this.lineRenderer.SetPosition(1, this.linePositions[1]);
    }

    //Deactivate LineDrawing so that no Line gets drawn when Cursor changes
    private void DeactivateLineDrawing()
    {
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.lineDrawingActivated = false;
    }
}
