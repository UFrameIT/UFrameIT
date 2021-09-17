using System.Collections;
using UnityEngine;
using static CommunicationEvents;
using TMPro;
using static Tape;

public class MenuScript : MonoBehaviour
{

    public enum MenuStates {None,One,Two,Three,Four,FactGenerating};
    public MenuStates currentState;

    public GameObject pageOne;
    public GameObject FactGenering;
    public GameObject pageTwo;
    public GameObject pageThree;
    public GameObject pageFour;
    public Problemobject problem;
    public GameObject charakterPrefab;
    public GameObject player;

    public static string desired_problemfact;

    public KeyCode Key = KeyCode.F1;

    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public MeshRenderer CursorRenderer;
    // integer check whether frameitUI is active or not
    public static int tabcounter;

    // "press key to talk" - reference
    


    // Start is called before the first frame update
    void Start()
    {
        currentState = MenuStates.None;
        charakterPrefab.GetComponentInChildren<CharacterDialog>().Key = KeyCode.M;
        tabcounter = 0;


    }

    // Update is called once per frame
    void Update()
    {
        //checks whether tab was pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabcounter++;
        }

        if (Input.GetKeyDown(Key) && currentState != MenuStates.Three)
        {
            if(currentState == MenuStates.None)
            {
                currentState = MenuStates.One;
            }
            else
            {
                currentState = MenuStates.None;
            }
        }

            switch (currentState)
        {
            case MenuStates.One:
                pageOne.SetActive(true);
                FactGenering.SetActive(false);
                pageTwo.SetActive(false);
                pageThree.SetActive(false);
                pageFour.SetActive(false);
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                Time.timeScale = 0f;
                break;

            case MenuStates.Two:
                pageOne.SetActive(false);
                FactGenering.SetActive(false);
                pageTwo.SetActive(true);
                pageThree.SetActive(false);
                pageFour.SetActive(false);
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                Time.timeScale = 0f;
                break;

            case MenuStates.Three:
                pageOne.SetActive(false);
                FactGenering.SetActive(false);
                pageTwo.SetActive(false);
                pageThree.SetActive(true);
                pageFour.SetActive(false);
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                Time.timeScale = 0f;
                break;

            case MenuStates.Four:
                pageOne.SetActive(false);
                FactGenering.SetActive(false);
                pageTwo.SetActive(false);
                pageThree.SetActive(false);
                pageFour.SetActive(true);
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                Time.timeScale = 0f;
                break;

            case MenuStates.None:
                pageOne.SetActive(false);
                FactGenering.SetActive(false);
                pageTwo.SetActive(false);
                pageThree.SetActive(false);
                pageFour.SetActive(false);
                if(tabcounter % 2 == 0)
                {
                    CamControl.enabled = false;
                    CursorRenderer.enabled = false;
                }
                else
                {
                    CamControl.enabled = true;
                    CursorRenderer.enabled = true;
                }
                Time.timeScale = 1f;
                break;

            case MenuStates.FactGenerating:
                pageOne.SetActive(false);
                FactGenering.SetActive(true);
                pageTwo.SetActive(false);
                pageThree.SetActive(false);
                pageFour.SetActive(false);
                if (tabcounter % 2 == 0)
                {
                    CamControl.enabled = false;
                    CursorRenderer.enabled = false;
                }
                else
                {
                    CamControl.enabled = true;
                    CursorRenderer.enabled = true;
                }
                Time.timeScale = 1f;
                break;
        }
        
    }

    public void OnAngle()
    {
        Tape.creatormodeON = true;
        currentState = MenuStates.FactGenerating;
        GetChildByName(pageTwo, "Actual desired scrolls").GetComponent<TMP_Text>().text = "SupplementaryAngles or AngleSum";
        GetChildByName(pageTwo, "Actual desired gadgets").GetComponent<TMP_Text>().text = "Pointer-Gadget, Angle-Gadget, Line-Gadget";
        desired_problemfact = "Angle";
        problem = new Problemobject();
    }

    public void OnPoint()
    {
        Tape.creatormodeON = true;
        currentState = MenuStates.FactGenerating;
        GetChildByName(pageTwo, "Actual desired scrolls").GetComponent<TMP_Text>().text = "MidPoint";
        GetChildByName(pageTwo, "Actual desired gadgets").GetComponent<TMP_Text>().text = "Pointer-Gadget";
        desired_problemfact = "Point";
        problem = new Problemobject();
    }

    public void OnDistance()
    {
        Tape.creatormodeON = true;
        currentState = MenuStates.FactGenerating;
        GetChildByName(pageTwo, "Actual desired scrolls").GetComponent<TMP_Text>().text = "OppositeLen";
        GetChildByName(pageTwo, "Actual desired gadgets").GetComponent<TMP_Text>().text = "Pointer-Gadget, Angle-Gadget, Line-Gadget";
        desired_problemfact = "Distance";
        problem = new Problemobject();
    }

    public void OnContinue()
    {
        Tape.creatormodeON = false;
        currentState = MenuStates.Two;
    }

    public void OnApply2()
    {
        //TODO: bei go-around-point den tatsächlichen goaround point so machen dass er am boden ist, damit npc nicht in der luft rumlaeuft
        // if two facts are in slots then go to next page and save the tho facts in the problemobject otherwise show error warning
        Fact problemFact = GetChildByName(pageTwo, "Slot 1").GetComponent<DropHandling>().currentFact;
        Fact goAroundFact = GetChildByName(pageTwo, "Slot 2").GetComponent<DropHandling>().currentFact;
        if(problemFact != null && goAroundFact != null && goAroundFact is PointFact)
        {
            switch (desired_problemfact)
            {
                case "Point":
                    if(problemFact is PointFact)
                    {
                        currentState = MenuStates.Three;
                        problem.problemFact = problemFact;
                        problem.goAroundPoint = goAroundFact;
                    }
                    else
                    {
                        Debug.Log("you need to select a Point Fact for the problem object!");
                    }
                    break;

                case "Angle":
                    if(problemFact is AngleFact)
                    {
                        currentState = MenuStates.Three;
                        problem.problemFact = problemFact;
                        problem.goAroundPoint = goAroundFact;
                    }
                    else
                    {
                        Debug.Log("you need to select a Angle Fact for the problem object!");
                    }
                    break;

                case "Distance":
                    if(problemFact is LineFact)
                    {
                        currentState = MenuStates.Three;
                        problem.problemFact = problemFact;
                        problem.goAroundPoint = goAroundFact;
                    }
                    else
                    {
                        Debug.Log("you need to select a Line Fact for the problem object!");
                    }
                    break;
            }
        }
        else
        {
            Debug.Log("Please make sure that both slots are filled and the go-around-object is a point fact!");

        }
        

    }

    public void OnApply3()
    {
        currentState = MenuStates.Four;
        problem.npcFailureText = "This is a test failure text";
        problem.npcProblemText = "This is a test problem description text";
        problem.npcSuccessText = "This is a test success text";
        GameObject inputField1 = GetChildByName(pageThree, "InputField (1)");
        GameObject inputField2 = GetChildByName(pageThree, "InputField (2)");
        if (inputField1 == null)
        {
            Debug.Log("inputfield1 nicht gefunden");
        }
        else
        {
            Debug.Log("inputfield gefunden");
        }

        string rawtext1 = inputField1.GetComponent<TMP_InputField>().text;
        string rawtext2 = inputField2.GetComponent<TMP_InputField>().text;
        int amountOfSentences1 = GetAmountOfSentences(rawtext1);

        Debug.Log("text aus eingabe ist: " + rawtext1);
        Debug.Log("Anzahl der Sätze ist: " + GetAmountOfSentences(rawtext1));

        // Setze die Saetze vom Taskcharakter
        charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences = new string[amountOfSentences1 + 1];  
        SetCharakterSentences(rawtext1, amountOfSentences1, charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences);
        charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences[amountOfSentences1] = rawtext2;

        Debug.Log("Anzahl Elemente von sentences ist " + charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences.Length);
        for(int i = 0; i<amountOfSentences1+1; i++)
        {
            if (charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences[i] != null)
            {
                Debug.Log("sentences[" + i + "] = " + charakterPrefab.GetComponentInChildren<CharacterDialog>().sentences[i].ToString());
            }
            else
            {
                Debug.Log("sentences[" + i + "] = null");
            }
        }

        //Get text from UI input
       // GameObject inputField1 = GetChildByName(pageThree, "InputField (1)");
        //string rawtext = inputField1.GetComponent<TextMesh>().text;

    }

    public void OnSave()
    {
        //Fenster wird nichtmehr angezeigt
        currentState = MenuStates.None;

        //Konvertiere Goaround pointfact zu Gameobject um es Taskcharakter zu uebergeben (y auf null damit er auf boden ist)
        GameObject walkAroundPoint = new GameObject(); // 
        float x = problem.goAroundPoint.Representation.transform.position.x;
        float y = 0;
        float z = problem.goAroundPoint.Representation.transform.position.z;
        walkAroundPoint.transform.position = new Vector3(x, y, z);

        //Setze WalkaroundObject von Taskcharakter
        charakterPrefab.GetComponent<TaskCharakterAnimation>().walkAroundObject = walkAroundPoint;

        //Setze Player von Taskcharakter
        charakterPrefab.GetComponent<TaskCharakterAnimation>().player = player;

        //Instantiate(charakterPrefab, new Vector3(x + (float)0.5, y + (float)0.5, z + (float)0.5), Quaternion.identity);
        GameObject realNewNpc = Instantiate(charakterPrefab, new Vector3(x, y, z), Quaternion.identity);

        //new NPC "press key to talk"-instantiation
        Transform myContinueText = realNewNpc.transform.Find("DialogManager/DialogText/ContinueText");
        GameObject myDialogManager = GetChildByName(realNewNpc, "DialogManager");
        myDialogManager.GetComponent<CharacterDialog>().continueText = myContinueText.gameObject;

        //new NPC Talking Zone initialization
        realNewNpc.GetComponent<TaskCharakterAnimation>().talkingZoneDistance = 10;

        //delete all previously generated facts (so that solution is not in the current factlist


        //CreateProblem();
    }

    public void OnBack1()
    {
        currentState = MenuStates.None;
    }

    public void OnBackFactGenering()
    {
        currentState = MenuStates.One;
    }

    public void OnBack2()
    {
        currentState = MenuStates.FactGenerating;
    }

    public void OnBack3()
    {
        currentState = MenuStates.Two;
    }

    public void OnBack4()
    {
        currentState = MenuStates.Three;
    }

    private T GetChildComponentByName<T>(string name, GameObject parent) where T : Component
    {
        foreach (T component in parent.GetComponentsInChildren<T>(true))
        {
            if (component.gameObject.name == name)
            {
                return component;
            }
        }
        return null;
    }

    private GameObject GetChildByName(GameObject parent, string name)
    {
        GameObject goal;
        int childrenLength = parent.transform.childCount;
        for(int i = 0; i<childrenLength; i++)
        {
            if(parent.transform.GetChild(i).gameObject.name == name)
            {
                return parent.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }

    private int GetAmountOfSentences(string text)
    {
        char[] textarray = text.ToCharArray();
        int amount = 0;
        for(int i = 0; i<textarray.Length; i++)
        {
            if(textarray[i] == '.' || textarray[i] == '?' || textarray[i] == '!')
            {
                amount++;
            }
        }
        return amount;
    }

    private void SetCharakterSentences(string text, int amountOfSentences, string[] sentences)
    { 
        char[] textarray = text.ToCharArray();
        //Debug.Log("textarray = " + textarray.ArrayToString() + "; laenge von textarray = " + textarray.Length);
        int pointCounter = 0;
        for(int i = 0; i<textarray.Length; i++)
        {
          if(textarray[i] != '.' && textarray[i] != '?' && textarray[i] != '!')
          {
                sentences[pointCounter] += textarray[i];
                //Debug.Log("pointcounter = " + pointCounter);
                //Debug.Log("textarray[" + i + "] = " + textarray[i]);
                
          }
          else
          {
                sentences[pointCounter] += textarray[i];
                pointCounter++;
          }

        }
        //Debug.Log("pointcounter = " + pointCounter);
    }

    /*private bool FrameIT_UI_Active()
    {
        return frameitUI.GetComponent<HideUI>.
    }*/
}
