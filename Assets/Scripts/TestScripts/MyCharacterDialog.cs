using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TaskCharakterAnimation;
using static FactOrganizer;

public class MyCharacterDialog : MonoBehaviour
{
    public TextMeshPro textDisplay;
    public TextMeshPro textHint;
    public TaskCharakterAnimation CharakterAnimation;
    public string[] sentences;
    private int sentenceIndex;
    private int letterIndex = 0;
    private bool typingActive = false;
    //speed for typing the Text
    public float typingSpeed;
    private float timer = 0;

    //Sebi: NPC ID
    public int npc_id;

    //Sebi: particle systems damit man sie mit invoke stoppen kann
    public GameObject successreaction;
    public GameObject failurereaction;

    //Sebi: npc reaction als string
    public string npcReaction;

    //Sebi: individuelles Problemobject für jeden NPC
    public GameObject problemObject;

    //Only reset once after Player is out of range of the TaskCharacter
    private bool textReseted = true;
    //If gameSucceeded -> Disable Talking with TaskCharacter
    private bool gameSucceeded = false;
    // choose talking key
    public KeyCode Key = KeyCode.F1;
    //Sebi: 2. Key für Solutionabfrage
    public KeyCode Key2 = KeyCode.M;
    //Sebi: testparameter um manuell funktionsbereitschaft der reaktionen zu testen 
    public bool testSolution = false;
    // parent gameobject where script is listed as a component
    public GameObject continueText;

    // Start is called before the first frame update
    void Start()
    {
        // initialize "press key to talk"-text
        continueText.GetComponent<TMP_Text>().text = "Press " + Key.ToString() + "-Key for Talking!" + "\n" + "Press " + Key2.ToString() + "-Key for checking your solution!";

        //Sebi auskommentiert
        //CommunicationEvents.gameSucceededEvent.AddListener(StartGameSucceededSentence);
        //CommunicationEvents.gameNotSucceededEvent.AddListener(StopGameSucceededSentence);

        //Type first sentence
        typingActive = true;
        TypeFkt();
    }

    private void Update()
    {
        TypeFkt();
        //Sebi modifiziert; factliste mit solution abgleichen (evtl nebenfunction TODO "boolean problemSolved()") 

        if (Input.GetKeyDown(Key) && CharakterAnimation.getPlayerInTalkingZone() && npc_id == CharakterAnimation.id)
        {
            //Type Next sentence if player is in the talkinZone around the TaskCharacter AND the player typed the return-Key
            NextSentence();
        }
        else if (!CharakterAnimation.getPlayerInTalkingZone() && !textReseted)
        {
            //Reset Sentence if Player is out of range of the TaskCharacter and it's not already reseted
            ResetSentence();
        }else if (CharakterAnimation.getPlayerInTalkingZone() && Input.GetKey(Key2) && npc_id == CharakterAnimation.id) // -> Solution erfragt
        {
            if (testSolution) // in if: problemsolved(); kann ich hier nicht einfügen sonst gibts compiling probleme
            {
                // activate success text
                StartSentenceAt(sentences.Length-2);
                // TODO activate success animation
                if(successreaction.name == "Fireworks_Animation")
                {
                    successreaction.transform.position = gameObject.transform.position;
                }
                successreaction.SetActive(true);

                // start npc success reaction
                gameObject.transform.parent.GetComponent<TaskCharakterAnimation>().startHappy();

                Invoke("StopSuccessReaction", (float)7.5);

                /*GameObject successInstantiation = Instantiate(problemObject.GetComponent<Problemobject>().successReaction);
                successreaction = successInstantiation.GetComponent<ParticleSystem>();
                successreaction.Play();
                Invoke("StopSuccessReaction", 3);*/


                //DestroyImmediate(successInstantiation, true);
                // TODO activate success npc reaction
            }
            else
            {
                // activate failure text
                StartSentenceAt(sentences.Length - 1);
                // TODO activate failure animation
                if(failurereaction.name == "Thunderbolt_Animation")
                {
                    failurereaction.transform.position = new Vector3(gameObject.transform.position.x, 40, gameObject.transform.position.z);
                }
                else if(failurereaction.name == "RainPrefab")
                {
                    
                    /*failurereaction.transform.position = gameObject.transform.position;
                    //failurereaction.SetActive(true);
                    for (int i = 0; i<3; i++)
                    {
                        failurereaction.transform.GetChild(i).transform.position = new Vector3(gameObject.transform.position.x, 40, gameObject.transform.position.z);
                        ParticleSystem ps = failurereaction.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                        var em = ps.emission;
                        em.enabled = true;
                    }
                    failurereaction.transform.GetChild(3).transform.position = new Vector3(gameObject.transform.position.x, 40, gameObject.transform.position.z);*/
                }
                failurereaction.SetActive(true);
                Invoke("StopFailureReaction", 5);

                /*GameObject failureInstantiation = problemObject.GetComponent<Problemobject>().failureReaction;
                failureInstantiation.name = "failureInstantiation";
                failurereaction = failureInstantiation.GetComponent<ParticleSystem>();
                failurereaction.Play();
                Invoke("StopFailureReaction", 3);*/

            }
        }
    }

    public void TypeFkt()
    {
        if (typingActive)
        {
            if (this.timer >= this.typingSpeed)
            {
                if (letterIndex < sentences[sentenceIndex].Length)
                {
                    textDisplay.text += sentences[sentenceIndex].ToCharArray()[letterIndex];
                    letterIndex++;
                }
                else
                {
                    this.typingActive = false;
                    letterIndex = 0;
                }

                this.timer = 0;
            }
            else
            {
                this.timer += Time.deltaTime;
            }
        }
    }


    public void NextSentence()
    {
        //-3 because the text before the last sentence is only for success text and the last sentence is for failure
        if (sentenceIndex < sentences.Length - 3)
        {
            CharakterAnimation.setTaskCharacterAddressed(true);
            sentenceIndex++;
            letterIndex = 0;
            typingActive = true;
            timer = 0;
            textDisplay.text = "";
            TypeFkt();
            textReseted = false;
        }
        else
        {
            letterIndex = 0;
            typingActive = false;
            timer = 0;
            textReseted = false;
            textDisplay.text = "";
        }
    }

    public void ResetSentence()
    {
        CharakterAnimation.setTaskCharacterAddressed(false);
        sentenceIndex = 0;
        letterIndex = 0;
        typingActive = true;
        timer = 0;
        textDisplay.text = "";
        //Type first sentence again
        TypeFkt();
        textReseted = true;
    }

    public void StartSentenceAt(int index)
    {
        //Sebi modifiziert game succeded variable unnötig
        //if (!gameSucceeded)
        //{
            textDisplay.text = "";
            //Last Sentence is the Pushout-Sentence
            sentenceIndex = index;
            letterIndex = 0;
            typingActive = true;
            timer = 0;
            gameSucceeded = true;
            //Disable Hint With Enter-Key for Talking
            textHint.GetComponent<MeshRenderer>().enabled = false;
            //Type final message
            TypeFkt();

            // Sebi neu
            //wait 3 seconds and stop game succeded sentence
            Invoke("StopGameSucceededSentence", 3);

        //}
    }

    public void StopGameSucceededSentence()
    {
        //Sebi modifiziert game succeded variable unnötig
        //if (gameSucceeded)
        //{
            gameSucceeded = false;
            //Enable Hint With Enter-Key for Talking
            textHint.GetComponent<MeshRenderer>().enabled = true;
            ResetSentence();

        //Sebi:
        textReseted = false;
        //}
    }

    public bool ProblemSolved()
    {
        Fact solution;
        return true;
    }

    public void StopSuccessReaction()
    {
        successreaction.SetActive(false);
    }

    public void StopFailureReaction()
    {
        failurereaction.SetActive(false);
    }
}
