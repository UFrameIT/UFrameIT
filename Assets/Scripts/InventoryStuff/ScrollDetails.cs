using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ScrollDetails : MonoBehaviour
{
    public WorldCursor cursor;
    public GameObject parameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public static List<GameObject> ParameterDisplays;

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items), 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cursor == null) cursor = GameObject.FindObjectOfType<WorldCursor>();

        CommunicationEvents.parameterDisplayHint.AddListener(animateScrollParameter);
        CommunicationEvents.newAssignmentEvent.AddListener(newAssignment);
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

    public void updateRenderedScroll(Scroll rendered)
    {
        Transform scroll = gameObject.transform.GetChild(1).transform;

        scroll.GetChild(0).GetComponent<TextMeshProUGUI>().text = rendered.description;
        for (int i = 0; i < rendered.requiredFacts.Count; i++) {
            var obj = ParameterDisplays.Find(x => x.transform.GetChild(0).GetComponent<RenderedScrollFact>().factUri.Equals(rendered.requiredFacts[i].@ref.uri));
            obj.transform.GetChild(0).GetComponent<RenderedScrollFact>().Label = rendered.requiredFacts[i].label;
        }
    }

    public void animateScrollParameter(string label)
    {
        var obj = ParameterDisplays.Find(x => x.transform.GetChild(0).GetComponent<RenderedScrollFact>().Label == label);
        obj.GetComponentInChildren<Animator>().SetTrigger("animateHint");
    }

    public void magicButton()
    {
        string answer = sendView("/scroll/apply");
        
        if (answer == null)
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            CommunicationEvents.PushoutFactFailEvent.Invoke(null);
            return;
        }
        List<Scroll.ScrollFact> pushoutFacts = JsonConvert.DeserializeObject<List<Scroll.ScrollFact>>(answer);
        readPushout(pushoutFacts);
    }

    public void newAssignment()
    {
        string answer = sendView("/scroll/dynamic");

        if (answer == null)
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            return;
        }
        Scroll.ScrollDynamicInfo scrollDynamicInfo = JsonConvert.DeserializeObject<Scroll.ScrollDynamicInfo>(answer);
        updateRenderedScroll(scrollDynamicInfo.rendered);
    }

    private string sendView(string endpoint)
    {
        string body = prepareScrollAssignments();

        UnityWebRequest www = UnityWebRequest.Put(CommunicationEvents.ServerAdress + endpoint, body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        var async = www.Send();
        while (!async.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            string answer = www.downloadHandler.text;
            Debug.Log(answer);
            return answer;
        }
    }

    private void readPushout(List<Scroll.ScrollFact> pushoutFacts)
    {
        FactManager factManager = cursor.GetComponent<FactManager>();
        for (int i = 0; i < pushoutFacts.Count; i++)
        {
            Fact newFact = ParsingDictionary.parseFactDictionary[pushoutFacts[i].getType()].Invoke(pushoutFacts[i]);
            int id = factManager.GetFirstEmptyID();
            newFact.Id = id;
            CommunicationEvents.Facts.Insert(id, newFact);
            CommunicationEvents.AddFactEvent.Invoke(newFact);
            CommunicationEvents.PushoutFactEvent.Invoke(newFact);
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
}
