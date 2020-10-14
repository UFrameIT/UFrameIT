using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
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

    public string situationTheory = "http://BenniDoes.Stuff?SituationTheory";

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items), 0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (cursor == null) cursor = GameObject.FindObjectOfType<WorldCursor>();

        _scrollDescriptionField = this.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setScroll(Scroll s)
    {
        Transform scrollView = gameObject.transform.GetChild(2);
        Transform viewport = scrollView.GetChild(0);
        this.scroll = s;
      
        ParameterDisplays = new List<GameObject>();
        Scroll.InitDynamicScroll(s.requiredFacts.Count);
        for (int i = 0; i < s.requiredFacts.Count; i++)
        {
            var obj = Instantiate(parameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);

            var scrollFact = obj.transform.GetChild(0).GetComponent<RenderedScrollFact>();

            scrollFact.ID = i;
            //obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            scrollFact.Label = Scroll.ParseString(i,s.requiredFacts[i].label,s.requiredFacts);
            obj.transform.SetParent(viewport.GetChild(0));
            //this is from benni, I dont know if the hack is the commented line below or the line above....
            //TODO: Remvoe this reaaaaly bad hack
            //obj.transform.localScale = Vector3.one;
            ParameterDisplays.Add(obj);
        }
        //scroll description
        ScrollDescription = Scroll.ParseString(-1, s.description, s.requiredFacts);
       
    }

    public void magicButton()
    {
        string view = sendView();
        if (view.Equals(FAIL))
        {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            CommunicationEvents.PushoutFactFailEvent.Invoke(null);
            return;
        }
        string ret = pushout(view);
        Debug.Log(ret);

    }

    string FAIL = "FAIL";
    class ViewResponse
    {
        public string view;
    }



    private string sendView()
    {

        List<Scroll.ScrollAssignment> assignments = new List<Scroll.ScrollAssignment>();
        Scroll.FilledScroll scroll = new Scroll.FilledScroll(this.scroll, assignments);
        string body = Scroll.ToJSON(scroll);

        UnityWebRequest www = UnityWebRequest.Put(CommunicationEvents.ServerAdress+"/scroll/apply", body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        var async = www.Send();
        while (!async.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return FAIL;
        }
        else
        {
            string answer = www.downloadHandler.text;
            return JsonUtility.FromJson<ViewResponse>(answer).view;
        }
    }
    /*
     *  private string sendView()
    {
        string jsonRequest = @"{";
        jsonRequest = jsonRequest + @" ""from"":""" + this.scroll.problemTheory + @""", ";
        jsonRequest = jsonRequest + @" ""to"":""" + this.situationTheory + @""", ";
        jsonRequest = jsonRequest + @" ""mappings"": { ";


        for (int i = 0; i < ParameterDisplays.Length; i++)
        {
            Fact fact_i = ParameterDisplays[i].GetComponentInChildren<DropHandling>().currentFact;
            var drophandler = ParameterDisplays[i].GetComponentInChildren<DropHandling>();
            Declaration decl_i = scroll.declarations[i];
            if (decl_i.value != null && fact_i.backendValueURI != null)
            {
                jsonRequest = jsonRequest + @" """ + decl_i.value + @""":""" + fact_i.backendValueURI + @""",";
            }
            jsonRequest = jsonRequest + @" """ + decl_i.identifier + @""":""" + fact_i.backendURI + @""",";
        }
        //removing the last ','
        jsonRequest = jsonRequest.Substring(0, jsonRequest.Length - 1);
        jsonRequest = jsonRequest + "}}";

        UnityWebRequest www = UnityWebRequest.Put("localhost:8081/view/add", jsonRequest);
        www.method = UnityWebRequest.kHttpVerbPOST;
        var async = www.Send();
        while (!async.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return FAIL;
        }
        else
        {
            string answer = www.downloadHandler.text;
            return JsonUtility.FromJson<ViewResponse>(answer).view;
        }
    }
    private string sendView2()
    {
        Dictionary<Declaration, Fact[]> possibilities = new Dictionary<Declaration, Fact[]>();
        for (int i = 0; i < ParameterDisplays.Length; i++)
        {
            Fact fact_i = ParameterDisplays[i].GetComponentInChildren<DropHandling>().currentFact;
            var drophandler = ParameterDisplays[i].GetComponentInChildren<DropHandling>();
            Declaration decl_i = scroll.declarations[i];
            if (fact_i is DirectedFact)
            {
                possibilities.Add(decl_i, new Fact[] { fact_i, ((DirectedFact)fact_i).flippedFact });
            }
            else
            {
                possibilities.Add(decl_i, new Fact[] { fact_i });
            }
        }
        Dictionary<Declaration, Fact> selection = new Dictionary<Declaration, Fact>();
        for (int i = 0; i < possibilities.Count; i++)
        {
            Declaration decl_i = new List<Declaration>(possibilities.Keys)[i];
            selection[decl_i] = possibilities[decl_i][0];
        }
        return testPossibilities(selection, possibilities, 0);

        return "";
    }

    private string testPossibilities(Dictionary<Declaration, Fact> selection, Dictionary<Declaration, Fact[]> possibilities, int i)
    {
        if (i == possibilities.Count)
        {
            return testVariant(selection);
        }
        else
        {
            Declaration decl_i = new List<Declaration>(possibilities.Keys)[i];
            for (int j = 0; j < possibilities[decl_i].Length; j++)
            {
                Fact fact_j = possibilities[decl_i][j];
                selection.Remove(decl_i);
                selection.Add(decl_i, fact_j);
                string ret = testPossibilities(selection, possibilities, i + 1);
                if (ret != FAIL) return ret;
            }
        }
        return FAIL;
    }

    private string testVariant(Dictionary<Declaration, Fact> selection)
    {
        string jsonRequest = @"{";
        jsonRequest = jsonRequest + @" ""from"":""" + this.scroll.problemTheory + @""", ";
        jsonRequest = jsonRequest + @" ""to"":""" + this.situationTheory + @""", ";
        jsonRequest = jsonRequest + @" ""mappings"": { ";
        for (int i = 0; i < selection.Count; i++)
        {
            Declaration decl_i = new List<Declaration>(selection.Keys)[i];
            Fact fact_i = selection[decl_i];
            if (decl_i.value != null && fact_i.backendValueURI != null)
            {
                jsonRequest = jsonRequest + @" """ + decl_i.value + @""":""" + fact_i.backendValueURI + @""",";
            }
            jsonRequest = jsonRequest + @" """ + decl_i.identifier + @""":""" + fact_i.backendURI + @""",";
        }
        //removing the last ','
        jsonRequest = jsonRequest.Substring(0, jsonRequest.Length - 1);
        jsonRequest = jsonRequest + "}}";

        UnityWebRequest www = UnityWebRequest.Put("localhost:8081/view/add", jsonRequest);
        www.method = UnityWebRequest.kHttpVerbPOST;
        var async = www.Send();
        while (!async.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return FAIL;
        }
        else
        {
            string answer = www.downloadHandler.text;
            return JsonUtility.FromJson<ViewResponse>(answer).view;
        }
    }*/


    class PushoutReturn
    {
        public string newSituation;
        public PushoutFact[] outputs;
    }

    [System.Serializable]
    public class PushoutFact
    {
        // generic class to make a common Supertype for all PushoutResponses
        public string uri;
        public string value;
        public string a;
        public string b;
        public string c;
        public string pointA;
        public string pointB;

        public string left;
        public string middle;
        public string right;

        public bool isVector()
        {
            return a != null &&
                b != null &&
                c != null &&
                pointA == null &&
                pointB == null &&
                value == null &&
                left == null &&
                middle == null &&
                right == null;
        }
        public bool isDistance()
        {
            return a == null &&
                b == null &&
                c == null &&
                pointA != null &&
                pointB != null &&
                value != null &&
                left == null &&
                middle == null &&
                right == null;
        }
        public bool isAngle()
        {
            return a == null &&
                b == null &&
                c == null &&
                pointA == null &&
                pointB == null &&
                value != null &&
                left != null &&
                middle != null &&
                right != null;
        }
    }

    private string pushout(string view)
    {
        string path = "localhost:8081/pushout?";
        path = path + "problem=" + this.scroll.problemTheory + "&";
        path = path + "solution=" + this.scroll.solutionTheory + "&";
        path = path + "view=" + view;
        UnityWebRequest www = UnityWebRequest.Get(path);
        var async = www.Send();
        while (!async.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            return FAIL;
        }
        else
        {
            string answer = www.downloadHandler.text;
            readPushout(answer);

            return "true";
            //return answer;
            //TODO Parse Anwser from JSON TO FACTS...
        }
    }

    private void readPushout(string txt)
    {
        Debug.Log(txt);
        PushoutReturn ret = JsonUtility.FromJson<PushoutReturn>(txt);
        this.situationTheory = ret.newSituation;
        FactManager factManager = cursor.GetComponent<FactManager>();
        for (int i = 0; i < ret.outputs.Length; i++)
        {
            PushoutFact f = ret.outputs[i];
            if (f.isVector())
            {
                float a = float.Parse(f.a);
                float b = float.Parse(f.b);
                float c = float.Parse(f.c);
                int id = factManager.GetFirstEmptyID();
                PointFact pf = new PointFact(id, a, b, c, f.uri);
                CommunicationEvents.Facts.Insert(id, pf);
                CommunicationEvents.AddFactEvent.Invoke(pf);
                CommunicationEvents.PushoutFactEvent.Invoke(pf);
            }
            if (f.isDistance())
            {
                int id = factManager.GetFirstEmptyID();
                int pid1 = getIdforBackendURI(f.pointA);
                int pid2 = getIdforBackendURI(f.pointB);
                LineFact lf = new LineFact();//id, pid1, pid2, f.uri, f.value);
                CommunicationEvents.Facts.Insert(id, lf);
                CommunicationEvents.AddFactEvent.Invoke(lf);
                CommunicationEvents.PushoutFactEvent.Invoke(lf);
            }
            if (f.isAngle())
            {
                int id = factManager.GetFirstEmptyID();
                int pid1 = getIdforBackendURI(f.left);
                int pid2 = getIdforBackendURI(f.middle);
                int pid3 = getIdforBackendURI(f.right);
                AngleFact af = new AngleFact();//id, pid1, pid2, pid3, f.uri, f.value);
                CommunicationEvents.Facts.Insert(id, af);
                CommunicationEvents.AddFactEvent.Invoke(af);
                CommunicationEvents.PushoutFactEvent.Invoke(af);
            }
        }
    }

    private int getIdforBackendURI(string uri)
    {
        return CommunicationEvents.Facts.Find(x => x.backendURI.Equals(uri)).Id;
    }
}
