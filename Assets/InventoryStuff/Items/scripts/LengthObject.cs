using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
    [CreateAssetMenu(fileName = "new DefaultObject", menuName = "Inventory System/Items/Length")]
public class LengthObject : ItemObject
{
    public string pointA;
    public string pointB;
    public double Lenght;


    public void Awake()
    {
       type = ItemType.LengthFact;
    }
    
     //prefab should be "LengthDisplay"
    public override  GameObject CreateDisplay(Transform transform, GameObject prefab){
        var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = pointA;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = pointB;
        return obj;
    }
}
