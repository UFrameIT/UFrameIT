using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class PoleTool : Gadget
    //Acts as a Pendulum starting at a Point
{
    //Attributes for simulating the drawing of a line
    public LayerMask LayerPendulumHits;
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    public Material linePreviewMaterial;

    public float poleHeight = 1f;
    public float maxHeight;

    new void Awake()
    {
        base.Awake();
        UiName = "PoleTool";
        if (MaxRange == 0)
            MaxRange = GlobalBehaviour.GadgetPhysicalDistance;
        if (maxHeight == 0)
            maxHeight = 0.1f;
    }

    new void OnEnable()
    {
        base.OnEnable();
        this.ResetGadget();
        ActivateLineDrawing();
    }

    void OnDisable()
    {
        this.ResetGadget();
    }

    public override void OnHit(RaycastHit hit)
    {

        if (!this.isActiveAndEnabled ||
            !Physics.Raycast(Cursor.transform.position + Vector3.up * (float)Math3d.vectorPrecission,
                Vector3.down, maxHeight + (float)Math3d.vectorPrecission, LayerMask.GetMask(new string[]{"Default", "Tree"})))
            return;

        UpdateLineDrawing();

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            var pid2 = FactManager.AddPointFact(linePositions[1], Vector3.up).Id;
            FactManager.AddLineFact(hit.transform.gameObject.GetComponent<FactObject>().URI, pid2, true);
        }
        else
        {
            FactManager.AddPointFact(hit);
        }
    }

    void Update()
    {
        if (!this.isActiveAndEnabled)
            return;

        if (lineRenderer.enabled)
            UpdateLineDrawing();
    }

    private void ResetGadget()
    {
        DeactivateLineDrawing();
    }

    private void ActivateLineDrawing()
    {
        this.lineRenderer.enabled = true;
        this.lineRenderer.positionCount = 2;
        this.lineRenderer.material = this.linePreviewMaterial;

        lineRenderer.startWidth = 0.095f;
        lineRenderer.endWidth = 0.095f;

        //initiate linePositions-Array
        linePositions.Add(this.Cursor.transform.position);
        linePositions.Add(this.Cursor.transform.position);

        UpdateLineDrawing();
    }

    //Updates the points of the Lines when baseLine was selected in LineMode
    private void UpdateLineDrawing()
    {
        this.linePositions[0] = this.Cursor.transform.position;

        //Raycast upwoard
        if (Physics.Raycast(this.linePositions[0], Vector3.up, out RaycastHit ceiling, poleHeight, this.LayerPendulumHits.value))
            this.linePositions[1] = ceiling.point;
        else
            this.linePositions[1] = this.linePositions[0] + Vector3.up * poleHeight;

        this.lineRenderer.SetPosition(0, this.linePositions[0]);
        this.lineRenderer.SetPosition(1, this.linePositions[1]);
    }

    //Deactivate LineDrawing so that no Line gets drawn when Cursor changes
    private void DeactivateLineDrawing()
    {
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.lineRenderer.enabled = false;
    }

}
