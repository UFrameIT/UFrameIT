using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void UpdateTagActive(this GameObject root, string tag, bool enable)
    {
        for (int i = 0; i < root.transform.childCount; i++)
        {
            GameObject child = root.transform.GetChild(i).gameObject;
            if (child.tag == tag)
                child.SetActive(enable);
            else
                UpdateTagActive(child, tag, enable);
        }
    }
}
