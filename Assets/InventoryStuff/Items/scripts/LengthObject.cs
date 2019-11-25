using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [CreateAssetMenu(fileName = "new DefaultObject", menuName = "Inventory System/Items/Length")]
public class LengthObject : ItemObject
{
    public string PointA;
    public string PointB;
    public double Lenght;


    public void Awake()
    {
       type = ItemType.LengthFact;
    }
}
