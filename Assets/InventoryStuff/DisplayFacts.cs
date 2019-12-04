using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayFacts : MonoBehaviour
{
    public Inventory inventory;

    public GameObject prefab_Point;
    public GameObject prefab_Distance;
    public GameObject prefab_Angle;
    public GameObject prefab_Default;

    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;
    // Start is called before the first frame update
    void Start()
    {
        //CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
       UpdateDisplay();
    }

    public void UpdateDisplay()
    {          
         for( int i = 0; i< inventory.Facts.Count; i++){
            if(! inventory.Facts[i].isDisplayed){
                var item = inventory.Facts[i].item;
                var obj = item.CreateDisplay(transform, getPrefab( item));
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                inventory.Facts[i].isDisplayed = true;
            }
            
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start+ (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
    }

    public GameObject getPrefab(ItemObject item){
        switch( item.type){
            case ItemObject.ItemType.LengthFact:return prefab_Distance; 
            case ItemObject.ItemType.AngleFact: return prefab_Angle;
            case ItemObject.ItemType.Point:     return prefab_Point;
            default:                            return prefab_Default;
        }
    }
}
