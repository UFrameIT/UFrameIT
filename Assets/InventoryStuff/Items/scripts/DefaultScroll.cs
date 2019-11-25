using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [CreateAssetMenu(fileName = "new DefaultScroll", menuName = "Inventory System/Items/DefaultScroll")]
public class DefaultScroll : ItemObject
{
    public void Awake()
    {
       type = ItemType.DefaultScroll;
    }
}
