using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static CommunicationEvents;
using System;

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
    public Material anglePreviewMaterial;
    
    private bool curveDrawingActivated;
    //These are only the vertices for the Curve
    private int curveDrawingVertexCount = 36;
    private LineFact curveDrawingStartLine;
    private Vector3 curveEndPoint;
    private Vector3 angleMiddlePoint;
    private float curveRadius;

    //Variables for Pushout-Highlighting
    private Fact highlightedPushoutFact;
    private GameObject extraHighlight;
    private bool timerActive { get; set; }
    private float timer { get; set; }
    private float timerDuration = 2.5f;
    private float timerDurationEnd = 5.0f;
    private int slowDownCount = 8;
    private int slowDownCounter;
    private bool[] slowDownSwitch;
    private float simulationSpeed;
    private Boolean pushoutFail;
    public GameObject directionalLight;
    private Color lightColor;
    private Color tempColor;
    private Color darkColor;
    private Boolean factAnimationActive = false;
    private float speedSlowDown;
    public Material pushoutMaterial;
    private Material tempMaterial;

    // Start is called before the first frame update
    public void Start()
    {
        if (Cursor == null) Cursor = GetComponent<WorldCursor>();
        CommunicationEvents.StartCurveDrawingEvent.AddListener(ActivateCurveDrawing);
        CommunicationEvents.StopCurveDrawingEvent.AddListener(DeactivateCurveDrawing);
        CommunicationEvents.StopPreviewsEvent.AddListener(StopPreviews);
        CommunicationEvents.PushoutFactEvent.AddListener(StartPushoutFactHighlighting);
        CommunicationEvents.PushoutFactFailEvent.AddListener(StartPushoutFactFailHighlighting);
        speedSlowDown = timerDurationEnd * 10;
        lightColor = directionalLight.GetComponent<Light>().color;

        slowDownSwitch = new bool[slowDownCount];
        Array.Clear(slowDownSwitch, 0, slowDownSwitch.Length);

        this.timerActive = false;
        this.timer = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        //SELECTION-HIGHLIGHTING-PART
        //Check if a Fact was Hit

        RaycastHit Hit = Cursor.Hit;

        Highlighting(Hit);

       
        //LineRendering-Part

        //@John before:  hit.point

        //Debug.Log(this.transform.position);
        
        if (this.curveDrawingActivated)
            UpdateCurveDrawing(this.transform.position);

        //If the Timer is Active, check Pushout-Highlighting
        if (this.timerActive)
        {
            this.timer += Time.deltaTime;
            CheckPushoutHighlighting();
        }
    }

    private void Highlighting(RaycastHit hit)
    {
        if (hit.transform != null)
        {
            Transform selection = hit.transform;

            //only do stuff if selection changes


            //Set the last Fact unselected
            if (this.lastFactSelection != null)
            {
                if (selection == this.lastFactSelection) return;
                //Invoke the EndHighlightEvent that will be handled in FactSpawner
                // CommunicationEvents.EndHighlightEvent.Invoke(this.lastFactSelection);
                if (this.lastFactSelection.CompareTag(selectableTag))
                    OnMouseOverFactEnd(lastFactSelection);
                else
                    OnMouseOverSnapZoneEnd(lastFactSelection);
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
            else if (selection.CompareTag("SnapZone"))
            {
                this.lastFactSelection = selection;
                OnMouseOverSnapZone(lastFactSelection);

            }
        }


        //SELECTION-HIGHLIGHTING-PART-END
    }

    private void OnMouseOverSnapZoneEnd(Transform selection)
    {
        Renderer selectionRenderer;

        if (selection != null)
        {
            selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                //Add transparency
                var oldCol = selectionRenderer.material.color;
                oldCol.a = .25f;
                selectionRenderer.material.color = oldCol;
                //Unhide Mouse cursor
                UnityEngine.Cursor.visible = true;
            }
        }
    }

    private void OnMouseOverSnapZone(Transform selection)
    {
        Renderer selectionRenderer;
        selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            //Remove transparency
            var oldCol = selectionRenderer.material.color;
            oldCol.a = .75f;
            selectionRenderer.material.color = oldCol;

            //Hide Mouse cursor
           UnityEngine.Cursor.visible = false;

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

    public void StartPushoutFactHighlighting(Fact startFact) {

        GameObject fireworksRepresentation = (GameObject)Resources.Load("Prefabs/Fireworks_Animation", typeof(GameObject));
        highlightedPushoutFact = startFact;

        if (typeof(PointFact).IsInstanceOfType(highlightedPushoutFact))
        {
            PointFact fact = (PointFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = pushoutMaterial;
            this.extraHighlight = GameObject.Instantiate(fireworksRepresentation);
            this.extraHighlight.transform.position = fact.Representation.transform.position;
        }
        else if (typeof(LineFact).IsInstanceOfType(highlightedPushoutFact))
        {
            LineFact fact = (LineFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = pushoutMaterial;
            this.extraHighlight = GameObject.Instantiate(fireworksRepresentation);
            this.extraHighlight.transform.position = fact.Representation.transform.position;
        }
        else if (typeof(AngleFact).IsInstanceOfType(highlightedPushoutFact)) {
            AngleFact fact = (AngleFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = pushoutMaterial;
            this.extraHighlight = GameObject.Instantiate(fireworksRepresentation);
            this.extraHighlight.transform.position = fact.Representation.transform.position;
        }

        //Activate Timer
        this.pushoutFail = false;
        this.slowDownCounter = 0;
        this.timerActive = true;
    }

    public void StopPushoutFactHighlighting() {

        if (typeof(PointFact).IsInstanceOfType(highlightedPushoutFact))
        {
            PointFact fact = (PointFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = tempMaterial;
        }
        else if (typeof(LineFact).IsInstanceOfType(highlightedPushoutFact))
        {
            LineFact fact = (LineFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = tempMaterial;
        }
        else if (typeof(AngleFact).IsInstanceOfType(highlightedPushoutFact))
        {
            AngleFact fact = (AngleFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = tempMaterial;
        }

        GameObject.Destroy(this.extraHighlight);
        this.extraHighlight = null;

        //Event for the happy Task-Charakter
        CommunicationEvents.PushoutFactEndEvent.Invoke(null);
}

    public void StartPushoutFactFailHighlighting(Fact startFact)
    {
        this.pushoutFail = true;
        this.tempColor = this.lightColor;
        this.darkColor = new Color(0.6f, 0.6f, 0.6f);
        this.timerActive = true;
    }

    public void CheckPushoutHighlighting() {
        //If the Pushout suceeded -> Fireworks-Animation
        if (this.pushoutFail == false)
        {
            //Fireworks already started in StartPushoutFactHighlighting
            if (this.timer >= this.timerDuration)
            {
                //After this.timerDuration+this.timerDurationEnd: Destroy Fireworks-Animation
                if (this.timer >= this.timerDuration + this.timerDurationEnd)
                {
                    this.timerActive = false;
                    this.timer = 0;
                    StopPushoutFactHighlighting();
                }
                //After this.timerDuration: Slow Down Fireworks
                else
                {
                    ParticleSystem main1 = this.extraHighlight.transform.GetChild(0).GetComponent<ParticleSystem>();
                    ParticleSystem main2 = this.extraHighlight.transform.GetChild(1).GetComponent<ParticleSystem>();
                    //Save StartSpeed when first slowing down
                    if (this.slowDownCounter == 0)
                        this.simulationSpeed = main1.main.simulationSpeed;
                    slowDownAnimation(main1, main2);
                }
            }
        }
        //If the Pushout failed -> Rain-Animation
        else
        {
            if (this.timer <= 0.5f * (this.timerDurationEnd))
            {
                //sky slowly gets dark
                if (directionalLight.GetComponent<Light>().color.r > darkColor.r)
                {
                    tempColor.r -= Time.deltaTime/5;
                    tempColor.g -= Time.deltaTime/5;
                    tempColor.b -= Time.deltaTime/5;
                    directionalLight.GetComponent<Light>().color = tempColor;
                }

            }
            else if (this.timer <= 2.0f * this.timerDuration + 0.5f * this.timerDurationEnd)
            {
                //Rain-Animation starts
                if (!factAnimationActive)
                {
                    GameObject RainRepresentation = (GameObject)Resources.Load("Prefabs/Rainmaker/RainPrefab", typeof(GameObject));
                    RainRepresentation.transform.position = new Vector3(0, 40, 0);
                    this.extraHighlight = GameObject.Instantiate(RainRepresentation);
                    factAnimationActive = true;
                }
            }
            //Rain-Animation stops and sky slowly gets bright again
            else if (this.timer <= 2.0f * this.timerDuration + this.timerDurationEnd)
            {
                if (factAnimationActive)
                {
                    //Stop Rain
                    GameObject.Destroy(this.extraHighlight);
                    this.extraHighlight = null;
                    factAnimationActive = false;
                }
                //sky slowly gets bright again
                if (directionalLight.GetComponent<Light>().color.r <= lightColor.r)
                {
                    tempColor.r += Time.deltaTime/5;
                    tempColor.g += Time.deltaTime/5;
                    tempColor.b += Time.deltaTime/5;
                    directionalLight.GetComponent<Light>().color = tempColor;
                }
            }
            else
            {
                //Stop timer
                this.timerActive = false;
                this.timer = 0;
            }
        }
    }

    public void slowDownAnimation(ParticleSystem main1, ParticleSystem main2) {

        if (this.timer <= this.timerDuration + (this.timerDurationEnd*((float)slowDownCounter+1.0f)/(float)slowDownCount))
        {
            if(slowDownSwitch[(int)((this.timer-this.timerDuration)/(this.timerDuration/(float)slowDownCount))] == false)
                if (slowDownCounter < slowDownCount)
                {
                    var mainModule1 = main1.main;
                    float speed = mainModule1.simulationSpeed;
                    mainModule1.simulationSpeed = speed - (float)(this.simulationSpeed / (float)slowDownCount);
                    var mainModule2 = main2.main;
                    mainModule2.simulationSpeed = speed - (float)(this.simulationSpeed / (float)slowDownCount);

                    slowDownSwitch[slowDownCounter] = true;
                }
        }
    }

    //Expect a LineFact here, where Line.Pid2 will be the Basis-Point of the angle
    public void ActivateCurveDrawing(Fact startFact)
    {
        //In AngleMode with 3 Points we want to draw nearly a rectangle so we add a startPoint and an Endpoint to this preview
        this.lineRenderer.positionCount = curveDrawingVertexCount + 2;
        this.lineRenderer.material = this.anglePreviewMaterial;

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        //Set CurveDrawing activated
        this.curveDrawingActivated = true;

        curveDrawingStartLine = (LineFact)startFact;
        PointFact curveDrawingPoint1 = (PointFact)Facts.Find(x => x.Id == curveDrawingStartLine.Pid1);
        PointFact curveDrawingPoint2 = (PointFact)Facts.Find(x => x.Id == curveDrawingStartLine.Pid2);

        //curveEndPoint is a point on the Line selected, with some distance from point2
        curveEndPoint = curveDrawingPoint2.Point + 0.3f * (curveDrawingPoint1.Point - curveDrawingPoint2.Point).magnitude * (curveDrawingPoint1.Point - curveDrawingPoint2.Point).normalized;
        
        angleMiddlePoint = curveDrawingPoint2.Point;
        curveRadius = (curveEndPoint - curveDrawingPoint2.Point).magnitude;
    }

    public void UpdateCurveDrawing(Vector3 currentPosition)
    {

        //Find the nearest of all potential third points
        PointFact nearestPoint = null;
        foreach (Fact fact in Facts) {
            if (fact is PointFact && fact.Id != curveDrawingStartLine.Pid1 && fact.Id != curveDrawingStartLine.Pid2 && nearestPoint == null)
                nearestPoint = (PointFact)fact;
            else if (fact is PointFact && fact.Id != curveDrawingStartLine.Pid1 && fact.Id != curveDrawingStartLine.Pid2 && (nearestPoint.Point - currentPosition).magnitude > (((PointFact)fact).Point - currentPosition).magnitude)
                nearestPoint = (PointFact)fact;
        }

        Vector3 startPoint = new Vector3(0,0,0);

        if (nearestPoint != null)
        {
            Vector3 planePoint = Vector3.ProjectOnPlane(currentPosition, Vector3.Cross((nearestPoint.Point-angleMiddlePoint), (curveEndPoint-angleMiddlePoint)));

            //Determine the Start-Point for the nearest third-point
            startPoint = angleMiddlePoint + curveRadius * (planePoint - angleMiddlePoint).normalized;
        }
        else
        {
            //Determine the Start-Point
            startPoint = angleMiddlePoint + curveRadius * (currentPosition - angleMiddlePoint).normalized;
        }

        //Determine the Center of Start-Point and End-Point 
        Vector3 tempCenterPoint = Vector3.Lerp(startPoint, curveEndPoint, 0.5f);
        Vector3 curveMiddlePoint = angleMiddlePoint + curveRadius * (tempCenterPoint - angleMiddlePoint).normalized;

        linePositions = new List<Vector3>();
        //Start: AngleMiddlePoint -> FirstPoint of Curve
        linePositions.Add(((PointFact)Facts.Find(x => x.Id == curveDrawingStartLine.Pid2)).Point);

        for (float ratio = 0; ratio <= 1; ratio += 1.0f / this.curveDrawingVertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPoint, curveMiddlePoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(curveMiddlePoint, curveEndPoint, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            linePositions.Add(bezierPoint);
        }

        //End: LastPoint of Curve -> AngleMiddlePoint
        linePositions.Add(((PointFact)Facts.Find(x => x.Id == curveDrawingStartLine.Pid2)).Point);

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
        
    }

    public void DeactivateCurveDrawing(Fact startFact)
    {
        
        this.lineRenderer.positionCount = 0;
        this.linePositions = new List<Vector3>();
        this.curveDrawingActivated = false;
    }

    public void StopPreviews(Fact startFact) {
        if (curveDrawingActivated)
            DeactivateCurveDrawing(null);
    }
}
