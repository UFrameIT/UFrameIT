using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [CreateAssetMenu(fileName = "new DefaultScroll", menuName = "Inventory System/Items/DefaultScroll")]
public class DefaultScroll : ItemObject
{

    public GameObject IconPrefab;
    public GameObject UsagePrefab;
    public void Awake()
    {
       type = ItemType.DefaultScroll;
    }

    //prefab should be "DefaultScrollDisplay"
    public override  GameObject CreateDisplay(Transform transform, GameObject prefab){
         var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
         return obj;
    }

}
