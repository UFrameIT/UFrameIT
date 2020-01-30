using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ScrollDetails : MonoBehaviour
{

    public GameObject parameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public GameObject[] ParameterDisplays;

    public string situationTheory = "http://BenniDoes.Stuff?SituationTheory";

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items ), 0f);
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
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.declarations[i].description;
            obj.transform.SetParent(viewport);
            this.ParameterDisplays[i] = obj;
        }
        gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.description;
    }

    public void magicButton() {
        string view = sendView();
        if (view.Equals(FAIL)) {
            Debug.Log("DAS HAT NICHT GEKLAPPT");
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
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
            jsonRequest = jsonRequest + @" """+decl_i.identifier +@""":""" + fact_i.backendURI + @""",";
            if (decl_i.value != null && fact_i.backendValueURI != null)
            {
                jsonRequest = jsonRequest + @" """ + decl_i.value + @""":""" + fact_i.backendValueURI + @""",";
            }
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
            return answer;
            //TODO Parse Anwser from JSON TO FACTS...
        }
    }
}
