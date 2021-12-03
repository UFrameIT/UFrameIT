using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TaskCharakterAnimation;
using static FactOrganizer;
using static FactComparer;

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

    //Sebi: Raininstance
    public GameObject rainInstance;
    //public GameObject rainPrefab;

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
        }else if (CharakterAnimation.getPlayerInTalkingZone() && Input.GetKeyDown(Key2) && npc_id == CharakterAnimation.id) // -> Solution erfragt
        {
            //if(StageStatic.stage.factState.ContainsKey(problemObject.GetComponent<Problemobject>().problemFact.Id))
            int counter = 0;
            bool solved = false;
            FactComparer Comparer = new FactEquivalentsComparer();
            Debug.Log("check " + StageStatic.stage.factState.FactDict.Values.Count.ToString() + "Facts in mode-factlist");
            foreach (Fact fact in StageStatic.stage.factState.FactDict.Values)
            {
                Debug.Log("counter = " + counter.ToString());
                // problemObject.GetComponent<Problemobject>().problemFact.GetType().Equals(fact.GetType()) &&
                //Comparer.Equals(fact, problemObject.GetComponent<Problemobject>().problemFact)
                // problemObject.GetComponent<Problemobject>().problemFact.Equals(fact)
                // ((fact is PointFact factpoint && solution is PointFact solutionpoint) || (fact is LineFact factline && solution is LineFact solutionline) || (fact is AngleFact factangle && solution is AngleFact solutionangle)) && Comparer.Equals(solution, fact)
                Fact solution = problemObject.GetComponent<Problemobject>().problemFact;
                if (FactIsEqual(fact, solution))
                {
                    Debug.Log("found right solution fact in factlist; fact nr:" + counter.ToString());
                    // activate success text
                    StartSentenceAt(sentences.Length - 2);
                    // TODO activate success animation
                    if (successreaction.name == "Fireworks_Animation")
                    {
                        successreaction.transform.position = gameObject.transform.position;
                    }
                    successreaction.SetActive(true);

                    // start npc success reaction
                    gameObject.transform.parent.GetComponent<TaskCharakterAnimation>().startHappy();

                    Invoke("StopSuccessReaction", (float)7.5);
                    solved = true;
                    break;
                }
                counter++;
            }
            if (!solved)
            {
                Debug.Log("found no right solution fact for problemobject");
                // activate failure text
                StartSentenceAt(sentences.Length - 1);
                // TODO activate failure animation
                if (failurereaction.name.Equals("Thunderbolt_Animation"))
                {
                    Debug.Log("failure reaction was " + failurereaction.name);
                    failurereaction.transform.position = new Vector3(gameObject.transform.position.x, 40, gameObject.transform.position.z);
                    failurereaction.SetActive(true);
                    Invoke("StopFailureReaction", 5);
                }
                else if (failurereaction.name.Equals("RainPrefab"))
                {
                    Debug.Log("enter rain case");
                    Debug.Log("failurereaction was " + failurereaction.name);
                    failurereaction.SetActive(true);
                    GameObject RainRepresentation = failurereaction;
                    RainRepresentation.transform.position = new Vector3(gameObject.transform.position.x, 40, gameObject.transform.position.z);
                    rainInstance = GameObject.Instantiate(RainRepresentation);
                    Debug.Log("raininstance succesfully set");
                    rainInstance.name = "raininstance";
                    
                    
                    Invoke("StopRain", 5);
                    failurereaction.SetActive(false);
               
                 
                }
                else
                {
        
                    Debug.Log("Failurereaction was " + failurereaction.name);
                    failurereaction.SetActive(true);
                    Invoke("StopFailureReaction", 5);
                }
             

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
            //CharakterAnimation.setTaskCharacterAddressed(true);
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

    public void StopRain()
    {
        GameObject.Destroy(rainInstance);
        rainInstance = null;
        Debug.Log("stopRain");
    }

    public bool FactIsEqual(Fact fact, Fact solution)
    {
        FactComparer Comparer = new FactEquivalentsComparer();
        if(fact is PointFact factpoint && solution is PointFact solutionpoint)
        {
            return Comparer.Equals(fact, solution);
        } else if(fact is AngleFact factangle && solution is AngleFact solutionangle)
        {
            if((factangle.Pid1 == solutionangle.Pid1 && factangle.Pid2 == solutionangle.Pid2 && factangle.Pid3 == solutionangle.Pid3) || (factangle.Pid1 == solutionangle.Pid3 && factangle.Pid2 == solutionangle.Pid2 && factangle.Pid3 == solutionangle.Pid1))
            {
                return true;
            } else
            {
                PointFact f1 = (PointFact) StageStatic.stage.factState.FactDict[factangle.Pid1];
                PointFact f2 = (PointFact) StageStatic.stage.factState.FactDict[factangle.Pid2];
                PointFact f3 = (PointFact) StageStatic.stage.factState.FactDict[factangle.Pid3];
                StageStatic.stage.SetMode(true);
                PointFact s1 = (PointFact)StageStatic.stage.factState.FactDict[solutionangle.Pid1];
                PointFact s2 = (PointFact)StageStatic.stage.factState.FactDict[solutionangle.Pid2];
                PointFact s3 = (PointFact)StageStatic.stage.factState.FactDict[solutionangle.Pid3];
                StageStatic.stage.SetMode(false);

                if((f1.Equivalent(s1) && f2.Equivalent(s2) && f3.Equivalent(s3)) || (f1.Equivalent(s3) && f2.Equivalent(s2) && f3.Equivalent(s1)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        } else if(fact is LineFact factline && solution is LineFact solutionline)
        {
            if ((factline.Pid1 == solutionline.Pid1 && factline.Pid2 == solutionline.Pid2) || (factline.Pid1 == solutionline.Pid2 && factline.Pid2 == solutionline.Pid1))
            {
                return true;
            }
            else
            {
                PointFact f1 = (PointFact)StageStatic.stage.factState.FactDict[factline.Pid1];
                PointFact f2 = (PointFact)StageStatic.stage.factState.FactDict[factline.Pid2];
                StageStatic.stage.SetMode(true);
                PointFact s1 = (PointFact)StageStatic.stage.factState.FactDict[solutionline.Pid1];
                PointFact s2 = (PointFact)StageStatic.stage.factState.FactDict[solutionline.Pid2];
                StageStatic.stage.SetMode(false);

                if ((f1.Equivalent(s1) && f2.Equivalent(s2)) || (f1.Equivalent(s2) && f2.Equivalent(s1)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }
}
