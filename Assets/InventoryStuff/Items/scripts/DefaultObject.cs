using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [CreateAssetMenu(fileName = "new DefaultObject", menuName = "Inventory System/Items/Default")]
public class DefaultObject : ItemObject
{
  public void Awake()
  {
      type = ItemType.Default;
  }

  //prefab should be "DefaultObjectDisplay"
    public override  GameObject CreateDisplay(Transform transform, GameObject prefab){
         var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
         return obj;
    }
}
