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
        sendView();
    }


    private void sendView() {
        string jsonRequest = @"{";
        jsonRequest = jsonRequest + @" ""from"":""" + this.scroll.problemTheory + @""", "; 
        jsonRequest = jsonRequest + @" ""to"":""" + this.situationTheory + @""", ";
        jsonRequest = jsonRequest + @" ""mappings"": { ";


        for (int i = 0; i < ParameterDisplays.Length; i++)
        {
            Fact fact_i = ParameterDisplays[i].GetComponentInChildren<DropHandling>().currentFact;
            Declaration decl_i = scroll.declarations[i];
            jsonRequest = jsonRequest + @" """+decl_i.identifier +@""":""" + fact_i.backendURI + @""",";
            if (decl_i.value != null && fact_i.backendValueURI != null) ;
            {
                jsonRequest = jsonRequest + @" """ + decl_i.value + @""":""" + fact_i.backendValueURI + @""",";
            }
        }
        //removing the last ','
        jsonRequest = jsonRequest.Substring(0, jsonRequest.Length - 1);
        jsonRequest = jsonRequest + "}}";
        Debug.Log(jsonRequest);
        /*
        UnityWebRequest www = UnityWebRequest.Post("localhost:8081/view/add", jsonRequest);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //TODO: hier ne Art PopUp, wo drin steht, dass das nicht geklappt hat
        }
        else
        {
            Debug.Log("Form upload complete!");
            string viewUri =  www.downloadHandler.text
        } */
    }
}
