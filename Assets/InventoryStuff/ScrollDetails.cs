using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollDetails : MonoBehaviour
{

    public GameObject ParameterDisplayPrefab;
    public Scroll scroll;

    public int x_Start;
    public int y_Start;
    public int y_Paece_Between_Items;

    public GameObject[] ParameterDisplays;

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start, y_Start + i * (-y_Paece_Between_Items ), 0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setScroll(Scroll s) {
        Transform scrollView = gameObject.transform.GetChild(2);
        Transform viewport = scrollView.GetChild(0);
        this.scroll = s;
        //wipe out old Displays
        for (int i = 0; i < this.ParameterDisplays.Length; i++) {
            Destroy(ParameterDisplays[i]);
        }
        this.ParameterDisplays = new GameObject[s.declarations.Length];
        for (int i = 0; i < s.declarations.Length; i++) {
            var obj = Instantiate(ParameterDisplayPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.declarations[i].description;
            obj.transform.SetParent(viewport);
            this.ParameterDisplays[i] = obj;
        }
        gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s.description;
    }

    public void magicButton() {
        for (int i = 0; i < ParameterDisplays.Length; i++) {
            Fact facti = ParameterDisplays[i].GetComponent<DropHandling>().currentFact;
            Declaration decl_i = scroll.declarations[i];
        }
    }
}
