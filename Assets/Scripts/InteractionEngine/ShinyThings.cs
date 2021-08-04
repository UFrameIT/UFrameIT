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
        if (Cursor == null) 
            Cursor = GetComponent<WorldCursor>();

        if (directionalLight == null)
            directionalLight = FindObjectOfType<Light>().gameObject;

        CommunicationEvents.PushoutFactEvent.AddListener(HighlightFact);
        CommunicationEvents.PushoutFactFailEvent.AddListener(StartPushoutFactFailHighlighting);
        CommunicationEvents.AnimateExistingFactEvent.AddListener(HighlightWithFireworks);

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

    public void HighlightWithFireworks(Fact fact)
    {
        GameObject fireworksRepresentation = (GameObject)Resources.Load("Prefabs/Fireworks_Animation", typeof(GameObject));

        this.extraHighlight = GameObject.Instantiate(fireworksRepresentation);
        this.extraHighlight.transform.position = fact.Representation.transform.position;

        HighlightFact(fact);
    }

    public void HighlightFact(Fact startFact) {

        highlightedPushoutFact = startFact;

        if (typeof(PointFact).IsInstanceOfType(highlightedPushoutFact))
        {
            PointFact fact = (PointFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = pushoutMaterial;
        }
        else if (typeof(LineFact).IsInstanceOfType(highlightedPushoutFact))
        {
            LineFact fact = (LineFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = pushoutMaterial;
        }
        else if (typeof(AngleFact).IsInstanceOfType(highlightedPushoutFact)) {
            AngleFact fact = (AngleFact)highlightedPushoutFact;
            tempMaterial = fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = pushoutMaterial;
        }

        //Activate Timer
        this.pushoutFail = false;
        this.slowDownCounter = 0;
        this.timerActive = true;
    }

    public void StopHighlighting() {

        if (typeof(PointFact).IsInstanceOfType(highlightedPushoutFact))
        {
            PointFact fact = (PointFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = tempMaterial;
        }
        else if (typeof(LineFact).IsInstanceOfType(highlightedPushoutFact))
        {
            LineFact fact = (LineFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = tempMaterial;
        }
        else if (typeof(AngleFact).IsInstanceOfType(highlightedPushoutFact))
        {
            AngleFact fact = (AngleFact)highlightedPushoutFact;
            fact.Representation.transform.GetChild(0).GetComponent<MeshRenderer>().material = tempMaterial;
        }

        if (this.extraHighlight != null)
        {
            GameObject.Destroy(this.extraHighlight);
            this.extraHighlight = null;
        }

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
                    StopHighlighting();
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
}
