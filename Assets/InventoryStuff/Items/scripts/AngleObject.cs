using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
