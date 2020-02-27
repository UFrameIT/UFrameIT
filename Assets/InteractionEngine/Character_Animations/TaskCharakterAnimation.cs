using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCharakterAnimation : MonoBehaviour
{
    public GameObject walkAroundObject;
    public GameObject player;
    public float radiusAroundObject;

    private Animator anim;
    private Transform currentTransform;
    private float currentDistance;

    private float walkingTime = 5;
    private bool walking = true;
    private float standingTime = 3;
    private bool standing = false;
    private float timer = 0;
    private bool rotate = false;
    private float nextRotation = 0;
    private float rotationUnits = 0;
    private float rotationTime = 2;
    private bool happy = false;

    private static bool playerInTalkingZone = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentTransform = GetComponent<Transform>();
        CommunicationEvents.PushoutFactEvent.AddListener(startHappy);
        CommunicationEvents.PushoutFactEndEvent.AddListener(stopHappy);
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing else than the animation if animationController is in happy-State
        if (happy)
            return;

        RaycastHit hit;
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        int layerMask = LayerMask.GetMask("TalkingZone"); //only hit TalkingZone

        //If Player is in TalkingZone: TaskCharacter should look to the Player, stop walking and enable enter-key for talking
        if (Physics.Raycast(ray, out hit, 5f, layerMask))
        {
            this.walking = false;
            this.standing = true;
            this.timer = 0;
            //Change boolean for switching to Standing-State in AnimationController
            anim.SetBool("standing", true);
            //Enable enter-key for talking
            setPlayerInTalkingZone(true);

            //Face walkAroundObject to Player (but only on y-Axis, so ignore x-and z-axis)
            currentTransform.LookAt(new Vector3(player.transform.position.x, currentTransform.position.y, player.transform.position.z));

            return;
        }
        else {
            //disable enter-key for talking
            setPlayerInTalkingZone(false);
        }

        //Calculate distance from tree, so that the TaskCharacter only walks in a specific radius around the tree
        //so that the player always knows where he is
        currentDistance = (currentTransform.position - walkAroundObject.transform.position).magnitude;

        //Turn on the radius-edges around the radiusAroundObject
        if (currentDistance > radiusAroundObject)
        {
            //Change roation if TaskCharacter is at the edges of the radius
            currentTransform.RotateAround(currentTransform.position, Vector3.up, 30 * Time.deltaTime);
            //return, because at the radius-edges we only want to rotate without standing/walking
            return;
        }

        this.timer += Time.deltaTime;

        if (rotate)
        {
            if (this.timer <= rotationTime)
            {
                //Change to random direction after standing-state
                currentTransform.RotateAround(currentTransform.position, Vector3.up, rotationUnits);
            }
            else {
                //After rotating: Change to Walking-State
                rotate = false;
                this.timer = 0;
                //Change boolean for switching to Walking-State in AnimationController
                anim.SetBool("standing", false);
            }
        }
        else
        {
            //If walking-time is over -> Change to standing-state
            if (walking && !standing)
            {
                if (this.timer >= this.walkingTime)
                {
                    this.walking = false;
                    this.standing = true;
                    this.timer = 0;
                    //Change boolean for switching to Standing-State in AnimationController
                    anim.SetBool("standing", true);
                }

            }
            else
            {
                //If standingTime is over: Change to walking-state, but before set rotate=true for rotating before walking
                if (this.timer >= this.standingTime)
                {
                    this.standing = false;
                    this.walking = true;
                    this.timer = 0;
                    //Rotate Character randomly
                    nextRotation = Random.Range(0, 180);
                    int positive = (int)Random.Range(0, 2);
                    if (positive == 0)
                        nextRotation *= -1;
                    rotationUnits = nextRotation / (rotationTime/Time.deltaTime);
                    rotate = true;
                }
            }
        }
    }

    public void startHappy(Fact startFact)
    {
        //Set Variable in animationController to change the state
        anim.SetBool("solved", true);
        happy = true;
    }

    public void stopHappy(Fact startFact) {
        //Set Variable in animationController to change the state
        anim.SetBool("solved", false);
        happy = false;
    }

    //Static Method for CharacterDialog
    public static bool getPlayerInTalkingZone() {
        return playerInTalkingZone;
    }

    //Static Method for CharacterDialog
    public static void setPlayerInTalkingZone(bool value) {
        playerInTalkingZone = value;
    }

}
