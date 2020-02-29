using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DisplayFacts : MonoBehaviour
{
    public HashSet<int> displayedFacts = new HashSet<int>();

    public GameObject prefab_Point;
    public GameObject prefab_Distance;
    public GameObject prefab_Angle;
    public GameObject prefab_Default;
    public GameObject prefab_OnLine;
    public GameObject prefab_Line;

    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;
    // Start is called before the first frame update
    void Start()
    {
        var rect = GetComponent<RectTransform>();
        x_Start = (int)(rect.rect.x + X_Pacece_Between_Items * .5f);
        y_Start = (int)(-rect.rect.y - y_Pacece_Between_Items * .5f);//);
        number_of_Column = Mathf.Max(1, (int)(rect.rect.width / prefab_Point.GetComponent<RectTransform>().rect.width) - 1);

        //CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay2();
    }

    public void UpdateDisplay2()
    {
        List<Fact>.Enumerator enumerator = CommunicationEvents.Facts.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int fid = enumerator.Current.Id;
            if (displayedFacts.Contains(fid))
            {
                continue;
            }
            var obj = CreateDisplay(transform, enumerator.Current);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(fid);
            displayedFacts.Add(fid);
        }

    }
    string getLetter(int Id) {
        return ((Char)(64 + Id + 1)).ToString();
    }

    private GameObject CreateDisplay(Transform transform, Fact fact)
    {
        switch (fact)
        {
            case LineFact f:
                {
                    var obj = Instantiate(prefab_Distance, Vector3.zero, Quaternion.identity, transform);
                    obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter( CommunicationEvents.Facts[f.Pid1].Id);
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid2].Id );
                    obj.GetComponent<FactWrapper>().fact = f;
                    return obj;
                }
            case RayFact f:
                {
                    var obj = Instantiate(prefab_Line, Vector3.zero, Quaternion.identity, transform);
                    obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(f.Id);
                    //obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid2].Id);
                    obj.GetComponent<FactWrapper>().fact = f;
                    return obj;
                }

            case AngleFact f:
                {
                    var obj = Instantiate(prefab_Angle, Vector3.zero, Quaternion.identity, transform);
                    obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid1].Id);
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid2].Id);
                    obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid3].Id);
                    obj.GetComponent<FactWrapper>().fact = f;
                    return obj;
                }

            case PointFact f:
                {
                    var obj = Instantiate(prefab_Point, Vector3.zero, Quaternion.identity, transform);
                    obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(f.Id );
                    obj.GetComponent<FactWrapper>().fact = f;
                    return obj;
                }
            case OnLineFact f:
                {
                    var obj = Instantiate(prefab_OnLine, Vector3.zero, Quaternion.identity, transform);
                    obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid].Id);
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Lid].Id);
                    obj.GetComponent<FactWrapper>().fact = f;
                    return obj;
                }



            default:
                {
                    var obj = Instantiate(prefab_Default, Vector3.zero, Quaternion.identity, transform);
                    return obj;
                }
           
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start + (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
    }

}
