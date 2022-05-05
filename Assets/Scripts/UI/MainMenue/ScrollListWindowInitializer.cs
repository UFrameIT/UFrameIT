using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollListWindowInitializer : MonoBehaviour
{
    void OnEnable()
    {
        var layoutElem = GetComponent<UnityEngine.UI.LayoutElement>();
        var parentRectTranform = transform.parent.GetComponent<RectTransform>();
        layoutElem.minHeight = parentRectTranform.rect.height;
    }
}
