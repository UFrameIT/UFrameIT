using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Tape : Gadget
{
    //Variables for LineMode distinction
    private bool TapeModeIsFirstPointSelected = false;
    private Fact TapeModeFirstPointSelected = null;

    //Attributes for simulating the drawing of a line
    private bool lineDrawingActivated;
    public WorldCursor Cursor;
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    public Material linePreviewMaterial;

    //Initialize Gadget when enabled AND activated
    void OnEnable()
    {
        this.ResetGadget();
    }

    void Start()
    {
        if (FactManager == null) FactManager = GameObject.FindObjectOfType<FactManager>();
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        if (this.Cursor == null) this.Cursor = GameObject.FindObjectOfType<WorldCursor>();
    }

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

            //we can only reach points that are lower than that with the measuring tape
            if (/*ActiveToolMode == ToolMode.CreateLineMode && */tempFact.Representation.transform.position.y > 2.5f)
                return;

            //If first point was already selected AND second point != first point
            if (this.TapeModeIsFirstPointSelected && this.TapeModeFirstPointSelected.Id != tempFact.Id)
            {
                this.DeactivateLineDrawing();
                //Create LineFact
                //Check if exactly the same line/distance already exists
                if (!FactManager.factAlreadyExists(new int[] { this.TapeModeFirstPointSelected.Id, tempFact.Id }))
                    //TODO: That won't work anymore because of gadget-refactoring
                    if (ActiveToolMode == ToolMode.CreateLineMode)
                        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddLineFact(this.TapeModeFirstPointSelected.Id, tempFact.Id, FactManager.GetFirstEmptyID()));
                    else
                    {
                        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddRayFact(this.TapeModeFirstPointSelected.Id, tempFact.Id, FactManager.GetFirstEmptyID()));

                    }

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
        /*
        //if we want to spawn a new point
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (this.TapeModeIsFirstPointSelected)
            {
            
                this.DeactivateLineDrawing();

                SmallRocket(hit, this.TapeModeFirstPointSelected.Id);

                this.ResetGadget();
            }
        }
        */
        //if we hit the top snap zone
        else if (hit.transform.gameObject.tag == "SnapZone")
        {
            if (this.TapeModeIsFirstPointSelected)
            {

                RaycastHit downHit;

                if (Physics.Raycast(hit.transform.gameObject.transform.position - Vector3.down * 2, Vector3.down, out downHit))
                {
                    int idA = downHit.transform.gameObject.GetComponent<FactObject>().Id;
                    int idB = this.TapeModeFirstPointSelected.Id;
                    int idC = FactManager.GetFirstEmptyID();
                    CommunicationEvents.AddFactEvent.Invoke(FactManager.AddPointFact(hit, idC));
                    this.DeactivateLineDrawing();
                    //Create LineFact
                    CommunicationEvents.AddFactEvent.Invoke(FactManager.AddAngleFact(idA, idB, idC, FactManager.GetFirstEmptyID()));
                    this.TapeModeIsFirstPointSelected = false;
                    this.TapeModeFirstPointSelected = null;
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
                this.DeactivateLineDrawing();
            }

            //TODO: Hint that only a line can be drawn between already existing points
        }
    }

    /*
    //Creating 90-degree Angles
    public void SmallRocket(RaycastHit hit, int idA)
    {
        //enable collider to measure angle to the treetop
        int idB = this.GetFirstEmptyID();
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddPointFact(hit, idB));
        Facts[idB].Representation.GetComponentInChildren<Collider>().enabled = true;
        //third point with unknown height
        int idC = FactManager.GetFirstEmptyID();
        var skyHit = hit;
        skyHit.point = (Facts[idA] as PointFact).Point + Vector3.up * 20;
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddPointFact(skyHit, idC));
        //lines
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddLineFact(idA, idB, this.GetFirstEmptyID()));
        //lines
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddLineFact(idA, idC, this.GetFirstEmptyID()));
        //90degree angle
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddAngleFact(idB, idA, idC, GetFirstEmptyID()));
    }*/

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
