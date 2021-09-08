using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldResizeFix : MonoBehaviour
{
    private Vector3 relPos;
    private float origHight;

    private void Start()
    {
        relPos = gameObject.transform.GetChild(0).transform.localPosition;
        origHight = (gameObject.transform as RectTransform).sizeDelta.y;
    }

    private void Update()
    {
        (gameObject.transform.GetChild(0) as RectTransform).sizeDelta =
            (gameObject.transform as RectTransform).sizeDelta;

        Fix();
    }

    public void Fix()
    {
        float newHight = (gameObject.transform as RectTransform).sizeDelta.y;
        gameObject.transform.GetChild(0).transform.localPosition = relPos
             - new Vector3(0, newHight - origHight, 0);
    }
}
