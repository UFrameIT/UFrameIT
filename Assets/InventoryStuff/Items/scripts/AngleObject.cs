using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
    [CreateAssetMenu(fileName = "new DefaultObject", menuName = "Inventory System/Items/Angle")]
public class AngleObject : ItemObject
{
    public string pointA;
    public string pointB;
    public string pointC;
    public double angle;

    public void Awake()
    {
       type = ItemType.AngleFact;
    }
    //prefab should be "AngleDisplay"
    public override  GameObject CreateDisplay(Transform transform, GameObject prefab){
        var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = pointA;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = pointB;
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = pointC;
        return obj;
    }
}
