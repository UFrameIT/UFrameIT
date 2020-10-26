﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class DisplayScrolls : MonoBehaviour
{
    public List<Scroll> scrolls;
    public GameObject[] ScrollButtons;
    public GameObject ScrollPrefab;
    public GameObject DetailScreen;



    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;


    // Update is called once per frame
    void Update()
    {

    }


    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start + (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
    }

  /*  [System.Serializable]
    class ScrollArrayWrapper {
        public Scroll[] Scrolls;
    };*/

    // Start is called before the first frame update
    void Start()
    {
        var rect = GetComponent<RectTransform>();
        x_Start = (int)(rect.rect.x + X_Pacece_Between_Items * .5f);
        y_Start = (int)(-rect.rect.y - y_Pacece_Between_Items * .5f);//);
        number_of_Column = Mathf.Max(1, (int)(rect.rect.width / ScrollPrefab.GetComponent<RectTransform>().rect.width) - 1);
        //get Scrolls from Backend;

        //string path = "Mock-Scrolls.json";
        //string jsonString = File.ReadAllText(path);
        //buildScrollSelection(jsonString);
        StartCoroutine(getScrollsfromServer());

    }

    IEnumerator getScrollsfromServer() {
        UnityWebRequest request = UnityWebRequest.Get(CommunicationEvents.ServerAdress + "/scroll/list");
        //Postman-Echo-Mock
        //UnityWebRequest request = UnityWebRequest.Get("https://019a8ea5-843a-498b-8d0c-778669aef987.mock.pstmn.io/get");
        request.method = UnityWebRequest.kHttpVerbGET;
        yield return request.Send();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning(request.error);
            string jsonString = File.ReadAllText(Application.dataPath + "/scrolls.json");
            Debug.Log(jsonString);
            BuildScrolls(jsonString);
        }
        else
        {
            CommunicationEvents.ServerRunning = true;
            string jsonString = request.downloadHandler.text;
            Debug.Log(jsonString);
            //scroll display not yet implemented;
            //buildScrollSelection(jsonString);
            BuildScrolls(jsonString);
        }
    }

    //new Protocol


    void BuildScrolls(string jsonString)
    {
        var scrolls = Scroll.FromJSON(jsonString);
        this.scrolls = scrolls;
        ScrollButtons = new GameObject[this.scrolls.Count];
        //Build Selection-GUI of Scrolls
        for (int i = 0; i < this.scrolls.Count; i++)
        {

            var obj = Instantiate(ScrollPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponent<ScrollClickedScript>().scroll = this.scrolls[i];
            obj.GetComponent<ScrollClickedScript>().DetailScreen = this.DetailScreen;
            obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.scrolls[i].label;
            ScrollButtons[i] = obj;
        }
        this.DetailScreen.GetComponent<ScrollDetails>().setScroll(this.scrolls[0]);
    }
    
   //old Protocol
   /*
    void buildScrollSelection(string jsonString) {
        jsonString = jsonString.Replace(System.Environment.NewLine, "");
        jsonString = jsonString.Replace("\t", "");

        ScrollArrayWrapper scrollsRead = new ScrollArrayWrapper();
        scrollsRead = (ScrollArrayWrapper)JsonUtility.FromJson(jsonString, scrollsRead.GetType());
        this.scrolls = scrollsRead.Scrolls;
        ScrollButtons = new GameObject[this.scrolls.Length];
        //Build Selection-GUI of Scrolls
        for (int i = 0; i < this.scrolls.Length; i++)
        {
            
            var obj = Instantiate(ScrollPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponent<ScrollClickedScript>().scroll = this.scrolls[i];
            obj.GetComponent<ScrollClickedScript>().DetailScreen = this.DetailScreen;
            obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.scrolls[i].label;
            ScrollButtons[i] = obj;
        }
        this.DetailScreen.GetComponent<ScrollDetails>().setScroll(this.scrolls[0]);


    }*/
}