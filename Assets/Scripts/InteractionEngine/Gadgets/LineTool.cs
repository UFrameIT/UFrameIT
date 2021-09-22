using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class LineTool : Gadget
{
    //Variables for LineMode distinction
    private bool LineModeIsFirstPointSelected = false;
    private Fact LineModeFirstPointSelected = null;

    //Attributes for simulating the drawing of a line
    private bool lineDrawingActivated;
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    public Material linePreviewMaterial;

    new void Awake()
    {
        base.Awake();
        UiName = "Line Mode";
        if (MaxRange == 0)
            MaxRange = GlobalBehaviour.GadgetLaserDistance;
    }

    //Initialize Gadget when enabled AND activated
    new void OnEnable()
    {
        base.OnEnable();
        this.ResetGadget();
    }

    void OnDisable()
    {
        this.ResetGadget();
    }

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            Fact tempFact = StageStatic.stage.factState[hit.transform.GetComponent<FactObject>().URI];

            //If first point was already selected AND second point != first point
            if (this.LineModeIsFirstPointSelected && this.LineModeFirstPointSelected.Id != tempFact.Id)
            {
                //Create LineFact
                FactManager.AddRayFact(this.LineModeFirstPointSelected.Id, tempFact.Id);

                this.ResetGadget();
            }
            else
            {
                //Activate LineDrawing for preview
                this.LineModeIsFirstPointSelected = true;
                this.LineModeFirstPointSelected = tempFact;
                this.ActivateLineDrawing();
            }
        }

        //if we hit the top snap zone
        //TODO: check behaviour
        else if (hit.transform.gameObject.CompareTag("SnapZone"))
        {
            if (this.LineModeIsFirstPointSelected)
            {

                RaycastHit downHit;

                if (Physics.Raycast(hit.transform.gameObject.transform.position - Vector3.down * 2, Vector3.down, out downHit))
                {
                    //Create LineFact
                    var idA = downHit.transform.gameObject.GetComponent<FactObject>().URI;
                    var idB = this.LineModeFirstPointSelected.Id;
                    FactManager.AddAngleFact(idA, idB, FactManager.AddPointFact(hit).Id);

                    this.ResetGadget();
                }
            }
        }

        //If no Point was hit
        else
        {
            if (this.LineModeIsFirstPointSelected)
            {
                //Deactivate LineDrawing and first point selection
                this.ResetGadget();
            }

            //TODO: Hint that only a line can be drawn between already existing points
        }
    }

    void Update()
    {
        if (!this.isActiveAndEnabled) return;
        if (this.lineDrawingActivated)
            UpdateLineDrawing();
    }

    private void ResetGadget()
    {
        this.LineModeIsFirstPointSelected = false;
        this.LineModeFirstPointSelected = null;
        DeactivateLineDrawing();
    }

    private void ActivateLineDrawing()
    {
        this.lineRenderer.enabled = true;
        this.lineRenderer.positionCount = 2;
        this.lineRenderer.material = this.linePreviewMaterial;

        lineRenderer.startWidth = 0.095f;
        lineRenderer.endWidth = 0.095f;
        //Set LineDrawing activated
        this.lineDrawingActivated = true;
        //Add the position of the Fact for the start of the Line
        linePositions.Add(this.LineModeFirstPointSelected.Representation.transform.position);
        //The second point is the same point at the moment
        linePositions.Add(this.LineModeFirstPointSelected.Representation.transform.position);

        this.lineRenderer.SetPosition(0, linePositions[0]);
        this.lineRenderer.SetPosition(1, linePositions[1]);

    }

    //Updates the second-point of the Line when First Point was selected in LineMode
    private void UpdateLineDrawing()
    {
        this.linePositions[1] = this.Cursor.transform.position;
        this.lineRenderer.SetPosition(1, this.linePositions[1]);
    }

    //Deactivate LineDrawing so that no Line gets drawn when Cursor changes
    private void DeactivateLineDrawing()
    {
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.lineDrawingActivated = false;
        this.lineRenderer.enabled = false;
    }
}
