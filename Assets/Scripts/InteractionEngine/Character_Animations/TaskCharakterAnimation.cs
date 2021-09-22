using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCharakterAnimation : MonoBehaviour
{
    public GameObject walkAroundObject;
    public GameObject player;
    public float radiusAroundObject;
    public float talkingZoneDistance;

    private Animator anim;
    private Transform currentTransform;
    private float currentDistance;

    //When changing walking/standing/happy booleans -> the state-variables in the animationController must also be changed
    //For change of the task-character movements, maybe the transitions in the animationController have also to be adjusted
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

    private bool LelvelVerifiedSolved = false;
    private float happyTimer = 0;
    private float happyTime = 7.5f;

    private bool playerInTalkingZone = false;
    private bool taskCharacterAddressed = false;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing else than the animation if animationController is in happy-State
        if (happy)
        {
            this.happyTimer += Time.deltaTime;
            if (happyTimer >= happyTime)
                stopHappy();
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        int layerMask = LayerMask.GetMask("TalkingZone"); //only hit TalkingZone

        //If Player is in TalkingZone: TaskCharacter should look to the Player, stop walking and enable enter-key for talking
        if (Physics.Raycast(ray, out hit, talkingZoneDistance, layerMask))
        {
            this.walking = false;
            this.standing = true;
            this.timer = 0;
            rotate = false;
            //Change boolean for switching to Standing-State in AnimationController
            anim.SetBool("standing", true);
            //Enable enter-key for talking for Charakter-Dialog
            setPlayerInTalkingZone(true);

            //Face walkAroundObject to Player (but only on y-Axis, so ignore x-and z-axis)
            currentTransform.LookAt(new Vector3(player.transform.position.x, currentTransform.position.y, player.transform.position.z));

            if(taskCharacterAddressed && !LelvelVerifiedSolved && checkGameSolved())
            {
                startHappy();
                LelvelVerifiedSolved = true;
            }
            taskCharacterAddressed = false;

            return;
        }
        else {
            //disable enter-key for talking
            setPlayerInTalkingZone(false);
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

                    //Calculate distance from tree, so that the TaskCharacter only walks in a specific radius around the tree
                    //so that the player always knows where he is
                    currentDistance = (currentTransform.position - walkAroundObject.transform.position).magnitude;
                    //Turn on the radius-edges around the radiusAroundObject
                    if (currentDistance > radiusAroundObject)
                    {
                        //Rotate Towards tree if radiusAroundObject is out of radius
                        //Rotate randomly between +/-5° towards the radiusAroundObject
                        int temp = Random.Range(0, 5);
                        int positive = (int)Random.Range(0, 2);
                        //Calculate NextRotation towards radiusAroundObject
                        nextRotation = Vector3.Angle(currentTransform.forward, (walkAroundObject.transform.position-currentTransform.position).normalized);

                        if (positive == 0)
                            nextRotation -= temp;
                        else
                            nextRotation += temp;
                    }
                    else
                    {
                        //Rotate Character randomly if radiusAroundObject is inside the radius
                        //Rotate inside the range of -180° and 180° because a normal human would turn e.g. -60° instead of 240°
                        nextRotation = Random.Range(0, 180);
                        int positive = (int)Random.Range(0, 2);
                        if (positive == 0)
                            nextRotation *= -1;
                    }

                    rotationUnits = nextRotation / (rotationTime/Time.deltaTime);
                    rotate = true;
                }
            }
        }
    }

    public bool checkGameSolved()
    {
        return StageStatic.stage.CheckSolved();
    }

    public void startHappy()
    {
        //Set Variable in animationController to change the state
        anim.SetBool("solved", true);
        happy = true;
        //Trigger for CharacterDialog
        CommunicationEvents.gameSucceededEvent.Invoke();
        //StartHappyTimer
        happyTimer = 0;
    }

    public void stopHappy()
    {
        //Set Variable in animationController to change the state
        anim.SetBool("solved", false);
        happy = false;
        resetToStart();
    }

    public void resetToStart() {
        //On Reset: Player must go into walking state, because it could be that after the happy/running-animation the player is 
        //out of radius and rotates -> AnimationController StateMachine always changes into walking-state after hyppAnimation
        walking = false;
        standing = true;
        rotate = false;
        nextRotation = 0;
        rotationUnits = 0;
        playerInTalkingZone = false;
        timer = 0;
    }

    //Static Method for CharacterDialog
    public bool getPlayerInTalkingZone() {
        return playerInTalkingZone;
    }

    //Static Method for CharacterDialog
    public void setPlayerInTalkingZone(bool value) {
        playerInTalkingZone = value;
    }

    //Static Method for CharacterDialog
    public bool getTaskCharacterAddressed()
    {
        return taskCharacterAddressed;
    }

    //Static Method for CharacterDialog
    public void setTaskCharacterAddressed(bool value)
    {
        taskCharacterAddressed = value;
    }

}
