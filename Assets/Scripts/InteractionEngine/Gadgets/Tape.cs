using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Tape : Gadget
{
    public float maxHeight = 2.5f;

    //Variables for TapeMode distinction
    private bool TapeModeIsFirstPointSelected = false;
    private Fact TapeModeFirstPointSelected = null;

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

        this.UiName = "Distance Mode";
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
            Fact tempFact = LevelFacts[hit.transform.GetComponent<FactObject>().URI];

            //we can only reach points that are lower than that with the measuring tape
            if (/*ActiveToolMode == ToolMode.CreateLineMode && */tempFact.Representation.transform.position.y > maxHeight)
                return;

            //If first point was already selected AND second point != first point
            if (this.TapeModeIsFirstPointSelected && this.TapeModeFirstPointSelected.Id != tempFact.Id)
            {
                //Create LineFact
                FactManager.AddLineFact(this.TapeModeFirstPointSelected.Id, tempFact.Id);

                this.ResetGadget();
            }
            else
            {
                //Activate LineDrawing for preview
                this.TapeModeIsFirstPointSelected = true;
                this.TapeModeFirstPointSelected = tempFact;
                this.ActivateLineDrawing();
            }
        }

        //if we hit the top snap zone
        //TODO: check behaviour
        else if (hit.transform.gameObject.CompareTag("SnapZone"))
        {
            if (this.TapeModeIsFirstPointSelected)
            {

                RaycastHit downHit;

                if (Physics.Raycast(hit.transform.gameObject.transform.position - Vector3.down * 2, Vector3.down, out downHit))
                {
                    var idA = downHit.transform.gameObject.GetComponent<FactObject>().URI;
                    var idB = this.TapeModeFirstPointSelected.Id;
                    var idC = FactManager.AddPointFact(hit).Id;
                    //Create LineFact
                    FactManager.AddAngleFact(idA, idB, idC, true);

                    this.ResetGadget();
                }
            }
        }
        //If no Point was hit
        else
        {
            if (this.TapeModeIsFirstPointSelected)
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
        this.TapeModeIsFirstPointSelected = false;
        this.TapeModeFirstPointSelected = null;
        DeactivateLineDrawing();
    }

    private void ActivateLineDrawing()
    {
        this.lineRenderer.positionCount = 2;
        this.lineRenderer.material = this.linePreviewMaterial;

        lineRenderer.startWidth = 0.095f;
        lineRenderer.endWidth = 0.095f;
        //Set LineDrawing activated
        this.lineDrawingActivated = true;
        //Add the position of the Fact for the start of the Line
        linePositions.Add(this.TapeModeFirstPointSelected.Representation.transform.position);
        //The second point is the same point at the moment
        linePositions.Add(this.TapeModeFirstPointSelected.Representation.transform.position);

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
    }
}
