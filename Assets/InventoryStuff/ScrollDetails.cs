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
    public GameObject cursor;
    public GameObject parameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public GameObject[] ParameterDisplays;

    public string situationTheory = "http://BenniDoes.Stuff?SituationTheory";

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items), 0f);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setScroll(Scroll s) {
        Transform scrollView = gameObject.transform.GetChild(2);
        Transform viewport = scrollView.GetChild(0);
        this.scroll = s;
        //wipe out old Displays
        for (int i = 0; i < this.ParameterDisplays.Length; i++) {
            Destroy(ParameterDisplays[i]);
        }
        this.ParameterDisplays = new GameObject[s.declarations.Length];
        for (int i = 0; i < s.declarations.Length; i++) {
            var obj = Instantiate(parameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            //obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.declarations[i].description;
            obj.transform.SetParent(viewport.GetChild(0));
            //TODO: Remvoe this reaaaaly bad hack
            //obj.transform.localScale = Vector3.one;
            this.ParameterDisplays[i] = obj;
        }
        gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.description;
    }

    public void magicButton() {
        string view = sendView();
        if (view.Equals(FAIL)) {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
            CommunicationEvents.PushoutFactFailEvent.Invoke(null);
            return;
        }
        string ret = pushout(view);
        Debug.Log(ret);

    }

    string FAIL = "FAIL";
    class ViewResponse {
        public string view;
    }

    private string sendView() {
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


    class PushoutReturn {
        public string newSituation;
        public PushoutFact[] outputs;
    }

    [System.Serializable]
    public  class PushoutFact {
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

        public bool isVector() {
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

    private string pushout(string view) {
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

    private void readPushout(string txt) {
        Debug.Log(txt);
        PushoutReturn ret = JsonUtility.FromJson<PushoutReturn>(txt);
        this.situationTheory = ret.newSituation;
        FactManager factManager = cursor.GetComponent<FactManager>();
        for (int i = 0; i < ret.outputs.Length; i++) {
            PushoutFact f = ret.outputs[i];
            if (f.isVector()) {
                float a = float.Parse(f.a);
                float b = float.Parse(f.b);
                float c = float.Parse(f.c);
                int id = factManager.GetFirstEmptyID();
                PointFact pf = new PointFact(id, a, b, c, f.uri);
                CommunicationEvents.Facts.Insert(id, pf);
                CommunicationEvents.AddFactEvent.Invoke(pf);
                CommunicationEvents.PushoutFactEvent.Invoke(pf);
            }
            if (f.isDistance()) {
                int id = factManager.GetFirstEmptyID();
                int pid1 = getIdforBackendURI(f.pointA);
                int pid2 = getIdforBackendURI(f.pointB);
                LineFact lf = new LineFact(id, pid1, pid2, f.uri, f.value);
                CommunicationEvents.Facts.Insert(id, lf);
                CommunicationEvents.AddFactEvent.Invoke(lf);
                CommunicationEvents.PushoutFactEvent.Invoke(lf);
            }
            if (f.isAngle()){
                int id = factManager.GetFirstEmptyID();
                int pid1 = getIdforBackendURI(f.left);
                int pid2 = getIdforBackendURI(f.middle);
                int pid3 = getIdforBackendURI(f.right);
                AngleFact af = new AngleFact(id, pid1, pid2, pid3, f.uri, f.value);
                CommunicationEvents.Facts.Insert(id, af);
                CommunicationEvents.AddFactEvent.Invoke(af);
                CommunicationEvents.PushoutFactEvent.Invoke(af);
            }
        }
    }

    private int getIdforBackendURI(string uri) {
        return CommunicationEvents.Facts.Find(x => x.backendURI.Equals(uri)).Id;
    }
}
