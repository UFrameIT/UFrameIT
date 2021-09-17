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

    public static void DestroyAllChildren(this GameObject root)
    {
        for (int i = 0; i < root.transform.childCount; i++)
            GameObject.Destroy(root.transform.GetChild(i).gameObject);
    }

    public static GameObject GetNthChild(this GameObject root, List<int> pos)
    {
        GameObject ret = root;
        foreach (var i in pos)
            ret = ret.transform.GetChild(i).gameObject;

        return ret;
    }
}
