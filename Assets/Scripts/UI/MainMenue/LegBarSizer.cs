using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Unused

public class LegBarSizer : MonoBehaviour
{
    public float offset = 5;
    private RectTransform transf;

    void Start()
    {
        transf = gameObject.GetComponent<RectTransform>();
        transf.sizeDelta = new Vector2(transf.sizeDelta.x, transf.sizeDelta.y + offset);
    }

    private void RecursiveTransform(List<GameObject> children)
    {
        ;
    }

    void Update()
    {
        foreach (var child in gameObject.GetComponentsInChildren<RectTransform>())
        {
            if (child.Equals(transf))
                continue;

            var pos = child.position;
            pos.y = transf.position.y + transf.sizeDelta.y - child.sizeDelta.y - offset;
            child.position = pos;
        }
    }
}
