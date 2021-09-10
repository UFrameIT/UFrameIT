using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenueLoader : MonoBehaviour
{
    public MenueLoader pageMenue;
    public UnityEngine.UI.ScrollRect scroll;
    public GameObject Pages;

    private int mode = 0;
    private int mode_last = 0;

    protected void Start()
    {
        if(scroll != null)
            scroll.verticalNormalizedPosition = 1f;
    }

    public void RevertMode()
    {
        SetMode(mode_last);
    }

    public void SetMode(int select)
    {
        switch (select)
        {
            case 0:
            case 1:
            case 2:
                break;

            case 3:
                //Pages.transform.GetChild(select).GetComponent<>
                break;
        }

        Clear();

        mode_last = mode;
        mode = select;

        Pages.transform.GetChild(select).gameObject.SetActive(true);
    }

    private void Clear()
    {
        for (int i = 0; i < Pages.transform.childCount; i++)
            Pages.transform.GetChild(i).gameObject.SetActive(false);
    }

}
