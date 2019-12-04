using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ItemObject : ScriptableObject
{
    public enum ItemType{
        LengthFact,
        AngleFact,
        DefaultScroll,
        Point,
        Default
    };
    public ItemType type;
    [TextArea(15,20)]
    public string Description;

    public abstract GameObject CreateDisplay(Transform transform, GameObject prefab);
}
