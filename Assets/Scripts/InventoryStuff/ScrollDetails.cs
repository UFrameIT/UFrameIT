using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static CommunicationEvents;

public class ScrollDetails : MonoBehaviour
{
    public WorldCursor cursor;
    public GameObject parameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public static List<GameObject> ParameterDisplays;
    private static List<Scroll.ScrollAssignment> LatestCompletions;

    public string currentMmtAnswer;

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items), 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cursor == null) cursor = GameObject.FindObjectOfType<WorldCursor>();

        parameterDisplayHint.AddListener(animateScrollParameter);
        CompletionsHintEvent.AddListener(animateCompletionsHint);
        NewAssignmentEvent.AddListener(newAssignmentTrigger);
    }

    public void setScroll(Scroll s)
    {
        Transform originalScroll = gameObject.transform.GetChild(1).transform;
        Transform originalScrollView = originalScroll.GetChild(1);
        Transform originalViewport = originalScrollView.GetChild(0);
        this.scroll = s;
        originalScroll.GetChild(0).GetComponent<TextMeshProUGUI>().text = s.description;

        //Clear all current ScrollFacts
        for (int i = 0; i < originalViewport.GetChild(0).childCount; i++) {
            GameObject.Destroy(originalViewport.GetChild(0).transform.GetChild(i).gameObject);
        }

        ParameterDisplays = new List<GameObject>();
        for (int i = 0; i < s.requiredFacts.Count; i++)
        {
            var originalObj = Instantiate(parameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            var originalScrollFact = originalObj.transform.GetChild(0).GetComponent<RenderedScrollFact>();
            originalScrollFact.ID = i;
            originalScrollFact.Label = s.requiredFacts[i].label;
            originalScrollFact.factUri = s.requiredFacts[i].@ref.uri;

            originalObj.transform.SetParent(originalViewport.GetChild(0));

            ParameterDisplays.Add(originalObj);
        }
    }

    public void magicButtonTrigger() {
        StartCoroutine(magicButton());
    }

    IEnumerator magicButton()
    {
        //Non blocking wait till sendView() is finished
        yield return sendView("/scroll/apply");

        if (currentMmtAnswer == null)
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            PushoutFactFailEvent.Invoke(null);
        }
        else
        {
            Scroll.ScrollApplicationInfo pushout = JsonConvert.DeserializeObject<Scroll.ScrollApplicationInfo>(currentMmtAnswer);
            readPushout(pushout.acquiredFacts);
        }
    }

    public void newAssignmentTrigger() {
        StartCoroutine(newAssignment());
    }

    IEnumerator newAssignment()
    {
        //Non blocking wait till sendView() is finished
        yield return sendView("/scroll/dynamic");

        if (currentMmtAnswer == null)
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
        }
        else
        {
            Scroll.ScrollDynamicInfo scrollDynamicInfo = JsonConvert.DeserializeObject<Scroll.ScrollDynamicInfo>(currentMmtAnswer);
            processScrollDynamicInfo(scrollDynamicInfo);
        }
    }

    IEnumerator sendView(string endpoint)
    {
        string body = prepareScrollAssignments();

        UnityWebRequest www = UnityWebRequest.Put(ServerAdress + endpoint, body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        var async = www.Send();
        while (!async.isDone) {
            //Non blocking wait for one frame, for letting the game do other things
            yield return null;
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            currentMmtAnswer = null;
        }
        else
        {
            string answer = www.downloadHandler.text;
            Debug.Log(answer);
            currentMmtAnswer = answer;
        }
    }

    private string prepareScrollAssignments()
    {
        Fact tempFact;
        List<Scroll.ScrollAssignment> assignmentList = new List<Scroll.ScrollAssignment>();

        for (int i = 0; i < ParameterDisplays.Count; i++)
        {
            Scroll.ScrollAssignment listEntry = new Scroll.ScrollAssignment();
            tempFact = ParameterDisplays[i].GetComponentInChildren<DropHandling>().currentFact;
            if (tempFact != null)
            {
                listEntry.fact = new Scroll.UriReference(this.scroll.requiredFacts[i].@ref.uri);
                listEntry.assignment = new JSONManager.OMS(tempFact.backendURI);
                assignmentList.Add(listEntry);
            }
        }

        Scroll.FilledScroll filledScroll = new Scroll.FilledScroll(this.scroll.@ref, assignmentList);
        return Scroll.ToJSON(filledScroll);
    }

    private void readPushout(List<Scroll.ScrollFact> pushoutFacts)
    {
        FactManager factManager = cursor.GetComponent<FactManager>();
        for (int i = 0; i < pushoutFacts.Count; i++)
        {
            Fact newFact = ParsingDictionary.parseFactDictionary[pushoutFacts[i].getType()].Invoke(pushoutFacts[i]);
            int id = factManager.GetFirstEmptyID();
            newFact.Id = id;
            Facts.Insert(id, newFact);
            AddFactEvent.Invoke(newFact);
            PushoutFactEvent.Invoke(newFact);
        }
    }

    public void processScrollDynamicInfo(Scroll.ScrollDynamicInfo scrollDynamicInfo) {

        if (scrollDynamicInfo.completions.Count != 0)
            LatestCompletions = scrollDynamicInfo.completions[0];
        else
            LatestCompletions = new List<Scroll.ScrollAssignment>();

        List<string> completionUris = new List<string>();
        foreach (Scroll.ScrollAssignment currentCompletion in LatestCompletions) {
            completionUris.Add(currentCompletion.fact.uri);
        }

        //Show that Hint is available for ScrollParameter
        HintAvailableEvent.Invoke(completionUris);

        updateRenderedScroll(scrollDynamicInfo.rendered);
    }

    public void updateRenderedScroll(Scroll rendered)
    {
        Transform scroll = gameObject.transform.GetChild(1).transform;

        scroll.GetChild(0).GetComponent<TextMeshProUGUI>().text = rendered.description;
        for (int i = 0; i < rendered.requiredFacts.Count; i++)
        {
            var obj = ParameterDisplays.Find(x => x.transform.GetChild(0).GetComponent<RenderedScrollFact>().factUri.Equals(rendered.requiredFacts[i].@ref.uri));
            obj.transform.GetChild(0).GetComponent<RenderedScrollFact>().Label = rendered.requiredFacts[i].label;
        }
    }

    public void animateCompletionsHint(GameObject scrollParameter, string scrollParameterUri) {
        Scroll.ScrollAssignment suitableCompletion = LatestCompletions.Find(x => x.fact.uri.Equals(scrollParameterUri) );

        if (suitableCompletion != null) {
            Fact fact = Facts.Find(x => x.backendURI.Equals(suitableCompletion.assignment.uri));
            if (fact != null) {
                //Animate ScrollParameter
                scrollParameter.GetComponentInChildren<Animator>().SetTrigger("animateHint");
                //Animate Fact in FactPanel
                AnimateExistingFactEvent.Invoke(fact);
                //Animate factRepresentation in game
                fact.Representation.GetComponentInChildren<Animator>().SetTrigger("animateHint");
            }
        }
    }

    public void animateScrollParameter(string label)
    {
        var obj = ParameterDisplays.Find(x => x.transform.GetChild(0).GetComponent<RenderedScrollFact>().Label == label);
        obj.GetComponentInChildren<Animator>().SetTrigger("animateHint");
    }
}
