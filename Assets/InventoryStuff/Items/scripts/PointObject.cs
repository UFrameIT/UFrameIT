using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
    [CreateAssetMenu(fileName = "new PointObject", menuName = "Inventory System/Items/Point")]
public class PointObject : ItemObject
{
    public string point;


    public void Awake()
    {
       type = ItemType.Point;
    }
    
     //prefab should be "LengthDisplay"
    public override  GameObject CreateDisplay(Transform transform, GameObject prefab){
        var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = point;
        return obj;
    }
}