using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static ParsingDictionary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;

public class ScrollDetails : MonoBehaviour
{
    public WorldCursor cursor;
    public GameObject parameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public static List<GameObject> ParameterDisplays;
    static TextMeshProUGUI _scrollDescriptionField;
    static string _scrollDescription;
    public static string ScrollDescription
    {
        get { return _scrollDescription; }
        set
        {
            _scrollDescription = value;
            _scrollDescriptionField.text = value;
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items), 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cursor == null) cursor = GameObject.FindObjectOfType<WorldCursor>();

        _scrollDescriptionField = this.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        CommunicationEvents.parameterDisplayHint.AddListener(animateScrollParameter);

    }

    public void setScroll(Scroll s)
    {
        Transform scrollView = gameObject.transform.GetChild(2);
        Transform viewport = scrollView.GetChild(0);
        this.scroll = s;

        //Clear all current ScrollFacts
        foreach (Transform child in viewport.GetChild(0).transform)
            GameObject.Destroy(child.gameObject);

        ParameterDisplays = new List<GameObject>();
        Scroll.InitDynamicScroll(s.requiredFacts.Count);
        for (int i = 0; i < s.requiredFacts.Count; i++)
        {
            var obj = Instantiate(parameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);

            var scrollFact = obj.transform.GetChild(0).GetComponent<RenderedScrollFact>();

            scrollFact.ID = i;
            //obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            scrollFact.Label = s.requiredFacts[i].label;
            obj.transform.SetParent(viewport.GetChild(0));
            ParameterDisplays.Add(obj);
        }
        //scroll description
        ScrollDescription = s.description;
       
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
