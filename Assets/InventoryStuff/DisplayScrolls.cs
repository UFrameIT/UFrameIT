using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayScrolls : MonoBehaviour
{
    public Scroll[] scrolls;
    public GameObject ScrollPrefab;



    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;
   

    // Update is called once per frame
    void Update()
    {
      // UpdateDisplay();
    }

    public void UpdateDisplay()
    {
       /*  for( int i = 0; i< inventory.Scrolls.Count; i++){
            if(! inventory.Scrolls[i].isDisplayed){
                var item = inventory.Scrolls[i].item;
                var obj = Instantiate(item.IconPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                inventory.Scrolls[i].isDisplayed = true;
            }
            
        }
        */
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start+ (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
    }

    [System.Serializable]
    class ScrollArrayWrapper{
        public Scroll[] scrolls;
    };

    // Start is called before the first frame update
    void Start()
    {
        //get Scrolls from Backend;
        //TODO REST-Call instead of Json-File
        string path = "Mock-Scrolls.json";
        string jsonString = File.ReadAllText(path);
        jsonString = jsonString.Replace(System.Environment.NewLine, "");
        jsonString = jsonString.Replace("\t", ""); 

        ScrollArrayWrapper scrollsRead = new ScrollArrayWrapper();
        scrollsRead = (ScrollArrayWrapper)JsonUtility.FromJson(jsonString, scrollsRead.GetType());
        this.scrolls = scrollsRead.scrolls;

        //Build Selection-GUI of Scrolls
        for (int i = 0; i < this.scrolls.Length; i++) {
            var obj = Instantiate(ScrollPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponent<ScrollClickedScript>().scroll = this.scrolls[i];
            obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.scrolls[i].label;
        }
    }
}
