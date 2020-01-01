using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScrolls : MonoBehaviour
{
    public List<Scroll> scrolls = new List<Scroll>();



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

    // Start is called before the first frame update
    void Start()
    {
        string path = "Mock-Scrolls.json";
        string jsonString = File.ReadAllText(path);
        jsonString = jsonString.Replace(System.Environment.NewLine, "");
        jsonString = jsonString.Replace("\t", "");
        Debug.Log(jsonString);
        Scroll[] scrollsRead = JsonUtility.FromJson<Scroll[]>(jsonString);
        //this.scrolls = scrollsRead;
    }
}
