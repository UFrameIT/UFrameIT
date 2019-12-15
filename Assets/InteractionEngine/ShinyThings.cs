using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinyThings : MonoBehaviour
{

    public WorldCursor Cursor;
    //Attributes for Highlighting of Facts when Mouse-Over
    private string selectableTag = "Selectable";
    private Transform lastFactSelection;
    public Material defaultMaterial;
    public Material highlightMaterial;


    //Attributes for simulating the drawing of a line
    public LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();
    private bool lineRendererActivated;

    // Start is called before the first frame update
    public void Start()
    {
        if(Cursor == null)Cursor = GetComponent<WorldCursor>();
       CommunicationEvents.StartLineRendererEvent.AddListener(ActivateLineRenderer);
       CommunicationEvents.StopLineRendererEvent.AddListener(DeactivateLineRenderer);
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
            if (this.lineRendererActivated)
                UpdateLineRenderer(Hit.point);
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

    public void ActivateLineRenderer(Fact startFact) {
        //Set LineRenderer activated
        this.lineRendererActivated = true;
        //Add the position of the Fact for the start of the Line
        linePositions.Add(startFact.Representation.transform.position);
        //The second point is the same point at the moment
        linePositions.Add(startFact.Representation.transform.position);

        this.lineRenderer.SetPosition(0, linePositions[0]);
        this.lineRenderer.SetPosition(1, linePositions[1]);

    }

    //Updates the second-point of the Line when First Point was selected in LineMode
    public void UpdateLineRenderer(Vector3 currentPosition)
    {
        this.linePositions[1] = currentPosition;
        this.lineRenderer.SetPosition(1, this.linePositions[1]);
    }

    //Deactivate LineRenderer so that no Line gets drawn when Cursor changes
    public void DeactivateLineRenderer(Fact startFact)
    {
        //Reset the first points
        this.lineRenderer.SetPosition(0, Vector3.zero);
        this.lineRenderer.SetPosition(1, Vector3.zero);
        if (linePositions.Count > 0)
            this.linePositions.Clear();
        this.lineRendererActivated = false;
    }
}
