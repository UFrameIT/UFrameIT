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

    }

    public void setScroll(Scroll s)
    {
        Transform originalScroll = gameObject.transform.GetChild(1).transform;
        Transform renderedScroll = gameObject.transform.GetChild(2).transform;

        Transform originalScrollView = originalScroll.GetChild(1);
        Transform renderedScrollView = renderedScroll.GetChild(1);
        Transform originalViewport = originalScrollView.GetChild(0);
        Transform renderedViewport = renderedScrollView.GetChild(0);
        this.scroll = s;

        originalScroll.GetChild(0).GetComponent<TextMeshProUGUI>().text = s.description;
        renderedScroll.GetChild(0).GetComponent<TextMeshProUGUI>().text = s.description;

        //Clear all current ScrollFacts
        for (int i = 0; i < originalViewport.GetChild(0).childCount; i++) {
            GameObject.Destroy(originalViewport.GetChild(0).transform.GetChild(i).gameObject);
            GameObject.Destroy(renderedViewport.GetChild(0).transform.GetChild(i).gameObject);
        }

        ParameterDisplays = new List<GameObject>();
        for (int i = 0; i < s.requiredFacts.Count; i++)
        {
            var originalObj = Instantiate(parameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            var originalScrollFact = originalObj.transform.GetChild(0).GetComponent<RenderedScrollFact>();
            originalScrollFact.ID = i;
            originalScrollFact.Label = s.requiredFacts[i].label;
            //Copy original Object for use in redered Scroll
            var renderedObj = Instantiate(originalObj);
            
            originalObj.transform.SetParent(originalViewport.GetChild(0));
            renderedObj.transform.SetParent(renderedViewport.GetChild(0));

            //Set bidirectional references for DropHandling
            var originalDropHandling = originalObj.transform.GetChild(0).GetComponent<DropHandling>();
            var renderedDropHandling = renderedObj.transform.GetChild(0).GetComponent<DropHandling>();
            originalDropHandling.associatedDropHandling = renderedDropHandling;
            renderedDropHandling.associatedDropHandling = originalDropHandling;

            ParameterDisplays.Add(originalObj);
        }

    }

    public void animateScrollParameter(string label)
    {
        var obj = ParameterDisplays.Find(x => x.transform.GetChild(0).GetComponent<RenderedScrollFact>().Label == label);
        Animator temp = obj.GetComponentInChildren<Animator>();
        temp.SetTrigger("animateHint");
    }

    public void magicButton()
    {
        List<Scroll.ScrollFact> pushoutFacts = sendView();
        if (pushoutFacts == null)
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            CommunicationEvents.PushoutFactFailEvent.Invoke(null);
            return;
        }
        readPushout(pushoutFacts);
    }

    private List<Scroll.ScrollFact> sendView()
    {
        string body = prepareScrollAssignments();

        UnityWebRequest www = UnityWebRequest.Put(CommunicationEvents.ServerAdress+"/scroll/apply", body);
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
            return JsonConvert.DeserializeObject<List<Scroll.ScrollFact>>(answer);
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
        List<List<System.Object>> assignmentList = new List<List<System.Object>>();

        for (int i = 0; i < ParameterDisplays.Count; i++)
        {
            List<System.Object> listEntry = new List<System.Object>();
            tempFact = ParameterDisplays[i].GetComponentInChildren<DropHandling>().currentFact;
            if (tempFact != null)
            {
                listEntry.Add(new JSONManager.URI(this.scroll.requiredFacts[i].@ref.uri));
                listEntry.Add(new JSONManager.OMS(tempFact.backendURI));
            }
            else
            {
                listEntry.Add(new JSONManager.URI(this.scroll.requiredFacts[i].@ref.uri));
                listEntry.Add(null);
            }
            assignmentList.Add(listEntry);
        }

        Scroll.FilledScroll filledScroll = new Scroll.FilledScroll(this.scroll.@ref, assignmentList);
        return Scroll.ToJSON(filledScroll);
    }
}
