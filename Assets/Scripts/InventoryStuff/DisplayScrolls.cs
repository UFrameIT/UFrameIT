using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DisplayScrolls : MonoBehaviour
{
    public string preferredStartScrollName;
    public int tryScrollListTimes = 2;

    public List<Scroll> scrolls;
    public GameObject[] ScrollButtons;
    public GameObject ScrollPrefab;
    public GameObject DetailScreen;

    public Transform scrollscreenContent;


    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;


    public Vector3 GetPosition(int i)
    {
        //return new Vector3(x_Start + (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
        return Vector3.zero;
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

    IEnumerator getScrollsfromServer()
    {
        //Try /scroll/listAll endpoint when scroll/list is not working
        UnityWebRequest request = UnityWebRequest.Get(CommunicationEvents.ServerAdress + "/scroll/list");
        //Postman-Echo-Mock
        //UnityWebRequest request = UnityWebRequest.Get("https://019a8ea5-843a-498b-8d0c-778669aef987.mock.pstmn.io/get");

        for (int i = 0; i < this.tryScrollListTimes; i++)
        {
            request = UnityWebRequest.Get(CommunicationEvents.ServerAdress + "/scroll/list");
            request.method = UnityWebRequest.kHttpVerbGET;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError
             || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning(request.error);
                Debug.Log("GET Scroll/list failed. Attempt: " + (i + 1).ToString());
            }
            else
            {
                break;
            }
        }

        if (request.result == UnityWebRequest.Result.ConnectionError
         || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogWarning(request.error);
            string jsonString = File.ReadAllText(Application.streamingAssetsPath + "/scrolls.json");
            Debug.Log(jsonString);
            BuildScrolls(jsonString);
        }
        else
        {
            CommunicationEvents.ServerRunning = true;
            string jsonString = request.downloadHandler.text;
            Debug.Log("JsonString from Server: \n" + jsonString);
            if (jsonString.Equals("[]"))
                jsonString = File.ReadAllText(Application.streamingAssetsPath + "/scrolls.json");
            Debug.Log("Used JsonString: \n" + jsonString);
            //scroll display not yet implemented;
            //buildScrollSelection(jsonString);
            BuildScrolls(jsonString);
        }
    }

    void BuildScrolls(string jsonString)
    {
        var scrolls = Scroll.FromJSON(jsonString);
        this.scrolls = scrolls;
        ScrollButtons = new GameObject[this.scrolls.Count];
        //Build Selection-GUI of Scrolls
        for (int i = 0; i < this.scrolls.Count; i++)
        {

            var obj = Instantiate(ScrollPrefab, Vector3.zero, Quaternion.identity, scrollscreenContent);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponent<ScrollClickedScript>().scroll = this.scrolls[i];
            obj.GetComponent<ScrollClickedScript>().DetailScreen = this.DetailScreen;
            obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.scrolls[i].label;
            ScrollButtons[i] = obj;
        }

        Scroll preferredStartScroll = this.scrolls.Find(x => x.label.Equals(preferredStartScrollName));
        if (preferredStartScroll != null)
            this.DetailScreen.GetComponent<ScrollDetails>().setScroll(preferredStartScroll);
    }
}
